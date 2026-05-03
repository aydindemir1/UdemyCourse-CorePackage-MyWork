using Core.Abstractions.Messaging.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Core.Messaging.Serialization
{
    // IMessageSerializer arayüzünün Newtonsoft.Json kütüphanesiyle gerçekleştirilmiş implementasyonu.
    // Genellikle sistemde varsayılan serializer olarak kullanılır.
    public class NewtonsoftJsonMessageSerializer : IMessageSerializer
    {
        public NewtonsoftJsonMessageSerializer()
        {
            // Parametresiz constructor (opsiyonel olarak DI'da kullanılabilir).
        }

        // Belirtilen JSON payload'ı generic tip olarak deserialize eder.
        public T? Deserialize<T>(string payload, bool camelCase = true)
        {
            return JsonConvert.DeserializeObject<T>(payload, CreateSerializerSettings(camelCase));
        }

        // Belirtilen JSON payload'ı verilen Type'a göre deserialize eder.
        public object? Deserialize(string payload, Type type, bool camelCase = true)
        {
            return JsonConvert.DeserializeObject(payload, type, CreateSerializerSettings(camelCase));
        }

        // Objeyi JSON string'e çevirir. CamelCase ve indented ayarlarını opsiyonel olarak destekler.
        public string Serialize(object obj, bool camelCase = true, bool indented = true)
        {
            return JsonConvert.SerializeObject(obj, CreateSerializerSettings(camelCase, indented));
        }

        // JsonSerializerSettings oluşturur. CamelCase ve indentation desteklenir.
        protected virtual JsonSerializerSettings? CreateSerializerSettings(bool camelCase = true, bool indented = false)
        {
            var settings = new JsonSerializerSettings();

            // Private setter'lara erişmek için özel contract resolver kullanılır.
            settings.ContractResolver = new ContractResolverWithPrivate();

            // JSON çıktısı satır satır yazılsın mı?
            if (indented)
            {
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            }

            // Private constructor olan class'lar için destek.
            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;

            // Recursive referanslar varsa kırılmasın diye ignore edilir.
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

            return settings;
        }

        // JSON.NET'te camelCase + private setter destekleyen custom resolver.
        private class ContractResolverWithPrivate : CamelCasePropertyNamesContractResolver
        {
            protected override Newtonsoft.Json.Serialization.JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                // Eğer property'si yazılabilir değilse ama private setter varsa yine de yazılabilir kabul et.
                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }
    }
}
