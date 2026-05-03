using Core.Abstractions.Messaging.Outbox;
using Core.Messaging.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Core.Messaging.Background
{
    // BackgroundService'ten türeyen bu sınıf, uygulama ayağa kalktığında otomatik başlar
    // ve uygulama kapanana kadar arka planda bir "Windows Servisi" gibi çalışır.
    public class OutboxProcessorBackgroundService : BackgroundService
    {
        private readonly bool _enabled; // Outbox aktif mi değil mi kontrolü için.
        private readonly TimeSpan _interval; // Her bir döngü arasında ne kadar uyuyacak?
        private readonly ILogger<OutboxProcessorBackgroundService> _logger; // Sistemin ne yaptığını kaydetmek için.

        // Scoped servisleri (DbContext, Repository) Singleton olan bu Worker içinde 
        // kullanabilmemizi sağlayan köprüdür (Çok kritik!).
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public OutboxProcessorBackgroundService(
            IServiceScopeFactory serviceScopeFactory,
            IOptions<OutboxOptions> outboxOptions,
            ILogger<OutboxProcessorBackgroundService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            // AppSettings'ten ayarları okuyoruz. Eğer boşsa varsayılan olarak 5 saniye ve 'Kapalı' diyoruz.
            _interval = outboxOptions.Value?.Interval ?? TimeSpan.FromSeconds(5);
            _enabled = outboxOptions.Value?.Enabled ?? false;
        }

        // .NET tarafından otomatik çağrılan ve asıl işin döndüğü ana metod.
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Eğer Outbox kapalıysa, boşuna döngüye girmeden servisi sonlandır.
            if (!_enabled)
            {
                _logger.LogInformation("Outbox is disabled");
                return;
            }

            _logger.LogInformation("Outbox is enabled");

            // stoppingToken iptal edilmediği sürece (yani uygulama kapanmadığı sürece) DÖN!
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogTrace("Started processing outbox messages...");

                // Performans ölçümü için kronometreyi başlatıyoruz.
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                // DİKKAT: BackgroundService Singleton'dır. IOutboxService ise Scoped'dır.
                // Scoped servise erişmek için manuel olarak bir 'Kapsam' (Scope) oluşturuyoruz.
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        // Oluşturduğumuz kapsamın içinden IOutboxService'i çekiyoruz.
                        var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();

                        // ASIL İŞ: Veritabanına gidip "gönderilmedi" işaretli mesajları bulur ve RabbitMQ'ya atar.
                        await outboxService.PublishUnsentOutboxMessagesAsync(stoppingToken);
                    }
                    catch (Exception exception)
                    {
                        // Herhangi bir hata (DB hatası, RabbitMQ kopması vb.) olursa döngüyü bozma, logla ve devam et.
                        _logger.LogError(
                            "There was an error when processing outbox, exception is: {Exception}",
                            exception.Message);
                    }
                } // using bloğu bittiği an, kullanılan servisler ve DB bağlantısı bellekten temizlenir.

                stopwatch.Stop();
                _logger.LogTrace("Finished processing outbox messages in {ElapsedMilliseconds} ms",
                    stopwatch.ElapsedMilliseconds);

                // Ayarlanan süre (örn: 5 saniye) boyunca servisi uyutuyoruz. 
                // stoppingToken ile uygulama kapanırsa uykunun bitmesini beklemeden anında durur.
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
