using Core.Abstractions.Events.External;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Messaging.Transport.RabbitMq
{
    public interface IMessageParser
    {
        /// <summary>
        /// Bir RabbitMQ mesajını (özellikler ve gövde) bir entegrasyon olayına (IIntegrationEvent) dönüştürür.
        /// </summary>
        /// <param name="basicProperties">Mesajın özelliklerini (örn: başlıklar) içeren nesne.</param>
        /// <param name="body">Mesajın ham içeriğini (gövdesini) içeren byte dizisi.</param>
        /// <returns>Çözümlenmiş entegrasyon olayı (IIntegrationEvent).</returns>
        IIntegrationEvent Resolve(IReadOnlyBasicProperties basicProperties, byte[] body);
    }
}
