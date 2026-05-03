using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Outbox
{
    // Outbox işlemleri için yapılandırma seçeneklerini temsil eder.
    // IConfiguration ile kolayca bind edilebilir.
    public class OutboxOptions
    {
        // Outbox için kullanılacak bağlantı string’i.
        public string ConnectionString { get; set; }

        // Outbox mekanizması aktif mi?
        public bool Enabled { get; set; }

        // Event işleme aralığı (örneğin her 5 saniyede bir outbox publish işlemi yapılacak).
        public TimeSpan? Interval { get; set; }

        // Background service kullanılacak mı (eğer kullanmak istemiyorsan elle tetikleyebilirsin).
        public bool UseBackgroundDispatcher { get; set; } = true;
    }
}
