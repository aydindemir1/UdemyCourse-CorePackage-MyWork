using Core.Abstractions.Messaging.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Background
{
    // Bu servis, sistemdeki tüm EventBus dinleyicilerini (Subscribers) tek bir merkezden yönetir.
    // BackgroundService olduğu için uygulama ayağa kalktığında otomatik olarak devreye girer.
    public class SubscribersBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider; // Kayıtlı tüm Subscriber'ları bulabilmek için.
        private readonly ILogger<SubscribersBackgroundService> _logger;

        public SubscribersBackgroundService(IServiceProvider serviceProvider, ILogger<SubscribersBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        // Worker (ve dolayısıyla uygulama) kapatılırken .NET tarafından çağrılır.
        // 'Graceful Shutdown' (Zarif Kapanış) dediğimiz olayı gerçekleştirir.
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Subsriber backgroundservice stopping...");

            // Dependency Injection konteynerındaki IEventBusSubscriber arayüzünden türeyen TÜM servisleri bulur.
            var subscribers = _serviceProvider.GetServices<IEventBusSubscriber>();

            // Tüm dinleyicilere "Kapatıyoruz, elindeki işi bitir ve bağlantını güvenlice kes" talimatı gönderir.
            // Task.WhenAll ile hepsinin aynı anda kapanış sürecini başlatırız.
            await Task.WhenAll(subscribers.Select(s => s.StopAsync(cancellationToken)));

            // Temel sınıftaki asıl durdurma mantığını çalıştırır.
            await base.StopAsync(cancellationToken);
        }

        // Worker başladığında bir kez çağrılır ve uygulama açık kaldığı sürece görevini sürdürür.
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Subsriber backgroundservice starting...");

            // Kayıtlı olan tüm Subscriber'ları (Örn: CatalogSubscriber, StockSubscriber vb.) getirir.
            var subscribers = _serviceProvider.GetServices<IEventBusSubscriber>();

            // Tüm dinleyicilerin StartAsync metodunu tetikleyerek RabbitMQ kuyruklarını dinlemeye başlatır.
            // Artık sistem dışarıdan gelecek event'lere kulak kabartmış durumdadır.
            return Task.WhenAll(subscribers.Select(s => s.StartAsync(stoppingToken)));
        }
    }
}
