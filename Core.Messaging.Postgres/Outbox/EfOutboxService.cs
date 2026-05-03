using Ardalis.GuardClauses;
using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Outbox;
using Core.Abstractions.Messaging.Serialization;
using Core.Abstractions.Messaging.Transport;
using Core.Messaging.Outbox;
using Core.Persistence.Contexts;
using Humanizer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Postgres.Outbox
{
    // PostgreSQL/Entity Framework Core kullanarak Outbox kayıtlarını yöneten ana servis.
    // TContext generic yapısı sayesinde farklı modüllerin DbContext'leri ile uyumlu çalışabilir.
    public class EfOutboxService<TContext> : IOutboxService where TContext : EfDbContextBase
    {
        private readonly OutboxOptions _options; // Çalışma aralığı ve aktiflik durumu ayarları.
        private readonly ILogger<EfOutboxService<TContext>> _logger;
        private readonly IMessageSerializer _messageSerializer; // C# nesnesini JSON'a (ve tersine) çeviren lojistik araç.
        private readonly IEventBusPublisher _eventBusPublisher; // RabbitMQ gibi dış dünyaya mesajı basan servis.
        private readonly OutboxDataContext _outboxDataContext; // Outbox tablolarına erişim sağlayan özel context.

        public EfOutboxService(
           IOptions<OutboxOptions> options,
           ILogger<EfOutboxService<TContext>> logger,
           IMessageSerializer messageSerializer,
           IEventBusPublisher eventBusPublisher,
           OutboxDataContext outboxDataContext)
        {
            _options = options.Value;
            _logger = logger;
            _messageSerializer = messageSerializer;
            _eventBusPublisher = eventBusPublisher;
            _outboxDataContext = outboxDataContext;

            // Geliştirme aşamasında sistemin ayarları doğru okuyup okumadığını görmek için kritik bir log.
            _logger.LogWarning("DEBUG - Outbox Enabled: {Enabled}", _options.Enabled);
        }

        // Veritabanında biriken ve işi bitmiş (gönderilmiş) eski kayıtları temizleyerek tabloyu hafifletir.
        public async Task CleanProcessedAsync(CancellationToken cancellationToken = default)
        {
            var messages = await _outboxDataContext.OutboxMessages.Where(x => x.ProcessedOn != null).ToListAsync();
            _outboxDataContext.OutboxMessages.RemoveRange(messages);
            await _outboxDataContext.SaveChangesAsync();
        }

        // Sistemde o an bulunan tüm outbox kayıtlarını listeler (Genelde admin panelleri veya debug için).
        public async Task<IEnumerable<OutboxMessage>> GetAllOutboxMessagesAsync(EventType eventType = EventType.IntegrationEvent, CancellationToken cancellationToken = default)
        {
            return await _outboxDataContext.OutboxMessages.Where(x => x.EventType == eventType).ToListAsync();
        }

        // Henüz sırası gelmemiş veya gönderilememiş (ProcessedOn == null) kayıtları filtreler.
        public async Task<IEnumerable<OutboxMessage>> GetAllUnsentOutboxMessageAsync(EventType eventType = EventType.IntegrationEvent, CancellationToken cancellationToken = default)
        {
            return await _outboxDataContext.OutboxMessages
                .Where(x => x.EventType == eventType && x.ProcessedOn == null)
                .ToListAsync();
        }

        // ASIL OPERASYON: BackgroundWorker tarafından tetiklenen, mesajları tek tek RabbitMQ'ya basan metod.
        public async Task PublishUnsentOutboxMessagesAsync(CancellationToken cancellationToken = default)
        {
            // 1. Veritabanından bekleyen mesajları çek.
            var unsentMessages = await _outboxDataContext.OutboxMessages
                .Where(x => x.ProcessedOn == null).ToListAsync();

            if (!unsentMessages.Any())
            {
                _logger.LogTrace("No unsent messages found in outbox");
                return;
            }

            foreach (var outboxMessage in unsentMessages)
            {
                // 2. Reflection kullanarak veritabanındaki string tip adından gerçek C# tipini bul.
                var type = Type.GetType(outboxMessage.Type);
                Guard.Against.Null(type, nameof(type));

                // 3. JSON veriyi tekrar canlı C# nesnesine (Event) dönüştür.
                dynamic? data = _messageSerializer.Deserialize(outboxMessage.Data, type);
                if (data is null)
                {
                    _logger.LogError("Invalid message type: {Name}", type?.Name);
                    continue;
                }

                // 4. Eğer nesne bir IntegrationEvent ise, RabbitMQ (veya ilgili Bus) üzerine publish et.
                if (data is IIntegrationEvent integrationEvent)
                {
                    await _eventBusPublisher.PublishAsync(integrationEvent);
                    _logger.LogInformation(
                      "Published a message: '{Name}' with ID: '{Id} (outbox)'",
                      outboxMessage.Name,
                      integrationEvent?.EventId);
                }

                // 5. Mesajın işlendiğini (gönderildiğini) işaretle (ProcessedOn tarihini atar).
                outboxMessage.MarkAsProcessed();
            }

            // 6. Döngü bittiğinde topluca veritabanını güncelle. 
            // Böylece "at-least-once delivery" (en az bir kere gönderim) garantisi sağlanır.
            await _outboxDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        // Tekli event kaydetme kolaylığı (Overload).
        public async Task SaveAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(integrationEvent, nameof(integrationEvent));
            await SaveAsync(new[] { integrationEvent }, cancellationToken);
        }

        // ASIL KAYIT: Business Logic ile aynı transaction içinde çalışan metod.
        public async Task SaveAsync(IIntegrationEvent[] integrationEvents, CancellationToken cancellationToken = default)
        {
            Guard.Against.Null(integrationEvents, nameof(integrationEvents));

            if (integrationEvents.Any() == false) return;

            // Eğer konfigürasyondan Outbox kapatıldıysa sadece log at ve veriyi kaydetme.
            if (!_options.Enabled)
            {
                _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
                return;
            }

            foreach (var integrationEvent in integrationEvents)
            {
                string name = integrationEvent.GetType().Name;

                // 1. Event nesnesini OutboxMessage entity'sine dönüştür (Mapping).
                // Nesneyi burada JSON'a (Serialize) çevirip saklıyoruz.
                var outboxMessage = new OutboxMessage(integrationEvent.EventId.ToString(),
                    integrationEvent.OccurredOn,
                    integrationEvent.EventType,
                    name.Underscore(), //standart bir format olan snake_case haline getirir. Humanizer kütüphanesi kullanılıyor.
                    _messageSerializer.Serialize(integrationEvent),
                    EventType.IntegrationEvent,
                    correlationId: integrationEvent.CorrelationId);

                // 2. Local DbContext'e ekle (Henüz veritabanına gitmedi).
                await _outboxDataContext.OutboxMessages.AddAsync(outboxMessage);
            }

            // 3. Veritabanına fiziksel olarak yaz. 
            // Eğer business logic patlarsa bu kayıtlar da kaydedilmeyecek (Atomicity).
            await _outboxDataContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved message to the outbox.");
        }
    }
}
