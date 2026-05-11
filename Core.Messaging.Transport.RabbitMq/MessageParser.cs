using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Serialization;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public class MessageParser : IMessageParser
    {
        // Sadece okunabilir (readonly) alanlar, sadece constructor içinde değer alabilir.
        private readonly IMessageSerializer _serializer; // Mesaj gövdesini (body) deseriyeleştirmek için kullanılır (örn: JSON'dan nesneye).


        /// <summary>
        /// MessageParser sınıfının yeni bir örneğini başlatır.
        /// </summary>
        /// <param name="serializer">Kullanılacak mesaj serileştirici.</param>
        /// <exception cref="ArgumentNullException">Eğer serializer veya typeResolver null ise bu hata fırlatılır.</exception>
        public MessageParser(IMessageSerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        /// <summary>
        /// Bir RabbitMQ mesajını (özellikler ve gövde) bir entegrasyon olayına (IIntegrationEvent) dönüştürür.
        /// </summary>
        /// <param name="basicProperties">Mesajın özelliklerini (örn: başlıklar) içeren nesne.</param>
        /// <param name="body">Mesajın ham içeriğini (gövdesini) içeren byte dizisi.</param>
        /// <returns>Çözümlenmiş entegrasyon olayı (IIntegrationEvent).</returns>
        /// <exception cref="ArgumentNullException">Eğer basicProperties veya body null ise bu hata fırlatılır.</exception>
        /// <exception cref="ArgumentException">Eğer mesaj tipi geçersizse veya çözümlenemiyorsa bu hata fırlatılır.</exception>
        public IIntegrationEvent Resolve(IReadOnlyBasicProperties basicProperties, byte[] body)
        {
            // Girdilerin null olup olmadığını kontrol eder.
            if (basicProperties is null)
                throw new ArgumentNullException(nameof(basicProperties));
            if (body is null)
                throw new ArgumentNullException(nameof(body));

            // Mesaj başlıklarının (headers) var olup olmadığını kontrol eder.
            if (basicProperties.Headers is null)
                throw new ArgumentNullException(nameof(basicProperties), "message headers are missing");

            // Başlıklardan "MessageType" anahtarıyla mesaj tipini almaya çalışır.
            // Tipin byte dizisi formatında olması beklenir.
            if (!basicProperties.Headers.TryGetValue(HeaderNames.MessageType, out var tmp) || tmp is not byte[] messageTypeBytes)
                throw new ArgumentException("invalid message type");

            // Byte dizisi olarak alınan mesaj tipi adını UTF8 formatında string'e çevirir.
            var messageTypeName = Encoding.UTF8.GetString(messageTypeBytes);

            // Tip çözümleyiciyi (type resolver) kullanarak string tip adından gerçek .NET Type nesnesini bulur.
            var dataType = Type.GetType(messageTypeName, throwOnError: false, ignoreCase: true);
            if (dataType is null)
                throw new ArgumentException("unable to detect message type from headers");

            // Mesajın gövdesini (body) UTF8 string'ine çevirir ve serileştirici (serializer) kullanarak ilgili .NET nesnesine dönüştürür (deserialization).
            var decodedObj = _serializer.Deserialize(Encoding.UTF8.GetString(body), dataType);

            // Dönüştürülen nesnenin beklenen IIntegrationEvent arayüzünü uygulayıp uygulamadığını kontrol eder.
            if (decodedObj is not IIntegrationEvent message)
                throw new ArgumentException($"message has the wrong type: '{messageTypeName}'");

            // Başarıyla çözümlenen mesaj nesnesini geri döndürür.
            return message;
        }
    }
}
