using Core.Abstractions.Events;
using Core.Abstractions.Events.External;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Core.Extensions.Types
{
    public static class TypeExtensions
    {

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
           // 'this Type openGenericType': Bu metodun bir 'Type' nesnesi üzerinde
           // genişletme metodu olarak çağrılmasını sağlar.
           // 'openGenericType' parametresi, aranacak olan açık jenerik arayüzü temsil eder.
           // Örneğin: typeof(IRequestHandler<,>) veya typeof(INotificationHandler<>)
           this Type openGenericType,
           // 'IEnumerable<Type> types': Bu parametre, tiplerin aranacağı koleksiyonu temsil eder.
           // Genellikle, uygulamanın yüklü tüm tipleri (AppDomain.CurrentDomain.GetAssemblies().SelectMany(a=> a.GetTypes()))
           // bu parametreye geçirilir.
           IEnumerable<Type> types)
        {
            // LINQ (Language Integrated Query) sorgusu başlatılır.
            // 'from type in types': 'types' koleksiyonundaki her bir 'Type' nesnesi üzerinde döngü yapar.
            return from type in types
                   // 'from interfaceType in type.GetInterfaces()': Her bir 'type' için,
                   // o tipin uyguladığı tüm arayüzler üzerinde döngü yapar.
                   from interfaceType in type.GetInterfaces()
                       // 'where' anahtar kelimesi, sonuçları filtrelemek için koşullar belirler.
                   where interfaceType.IsGenericType // 1. Koşul: Arayüzün jenerik bir tip olup olmadığını kontrol eder.
                                                     // (Örn: IEnumerable<T> jeneriktir, string değildir.)

                        // 2. Koşul: Bu, metodun ana mantığıdır.
                        // 'interfaceType.GetGenericTypeDefinition()': Eğer 'interfaceType' jenerik bir arayüzün somut bir implementasyonu ise
                        // (örn: List<string> için IEnumerable<string>), bu metot o jenerik arayüzün açık jenerik tanımını döndürür
                        // (örn: IEnumerable<>).
                        // 'openGenericType.IsAssignableFrom(...)': 'openGenericType' (aranan açık jenerik arayüz, örn: IRequestHandler<,>)
                        // 'interfaceType.GetGenericTypeDefinition()' (bulunan arayüzün açık jenerik tanımı, örn: IRequestHandler<TRequest, TResponse>)
                        // ile uyumlu olup olmadığını kontrol eder.
                        // Yani, 'type'ın uyguladığı jenerik arayüzün, bizim aradığımız 'openGenericType' ile aynı jenerik tanıma sahip olup olmadığını kontrol eder.
                        && openGenericType.IsAssignableFrom(interfaceType.GetGenericTypeDefinition()) && type.IsClass && !type.IsAbstract
                   select type;

            // 3. Koşul: Bulunan 'type'ın bir sınıf olup olmadığını kontrol eder.
            // Bu, arayüzleri veya yapıları (struct) elemek içindir. && type.IsClass

            // 4. Koşul: Bulunan 'type'ın soyut (abstract) bir sınıf olup olmadığını kontrol eder.
            // '!type.IsAbstract' demek, sınıfın somut (concrete) ve örneklenebilir olmasıgerektiği anlamına gelir. && !type.IsAbstract
            // 'select type': Yukarıdaki tüm koşulları sağlayan 'type' nesnelerini sonuç olarakdöndürür select type;
        }

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
            this Type openGenericType,
            Assembly assembly)
        {
            try
            {
                return GetAllTypesImplementingOpenGenericInterface(openGenericType, assembly.GetTypes());
            }
            catch (ReflectionTypeLoadException)
            {
                return Enumerable.Empty<Type>();
            }
        }

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericInterface(
           this Type openGenericType,
           params Assembly[] assemblies)
        {
            var inputAssemblies = assemblies.Any() ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
            return inputAssemblies.SelectMany(assembly =>
                GetAllTypesImplementingOpenGenericInterface(openGenericType, assembly));
        }

        public static IEnumerable<Type> GetHandledIntegrationEventTypes(
            // 'this Assembly[] assemblies': Bu metodun bir 'Assembly[]' dizisi üzerinde
            // genişletme metodu olarak çağrılmasını sağlar.
            // 'assemblies' parametresi, tiplerin aranacağı Assembly'ler koleksiyonunu temsil eder.
            // Genellikle, uygulamanın yüklü tüm Assembly'leri (AppDomain.CurrentDomain.GetAssemblies())
            // bu parametreye geçirilir.
            this Assembly[] assemblies)
        {
            // 1. Adım: Belirli bir açık jenerik arayüzü (IIntegrationEventHandler<>) uygulayan tüm somut   sınıfları bul.
            // typeof(IIntegrationEventHandler<>): Aranacak olan açık jenerik arayüzün tanımı.
            // .GetAllTypesImplementingOpenGenericInterface(assemblies): Daha önce açıkladığımız genişletme   metodu.
            // Bu adımın sonucu: IIntegrationEventHandler<TEvent> arayüzünü uygulayan tüm somut handler sınıfları (örn: OrderCreatedEventHandler, ProductUpdatedEventHandler).
            return typeof(IIntegrationEventHandler<>).GetAllTypesImplementingOpenGenericInterface(assemblies)
               // 2. Adım: Bulunan her handler sınıfının uyguladığı tüm arayüzleri al.
               // Neden? Çünkü biz handler sınıfının kendisini değil, onun uyguladığı  IIntegrationEventHandler<TEvent> arayüzünü ve
               // bu arayüzün jenerik argümanı olan TEvent'i (yani işlediği olay tipini) istiyoruz.
               .SelectMany(x => x.GetInterfaces())
                 // 3. Adım: Sadece IIntegrationEventHandler<> arayüzünün somut implementasyonlarını filtrele.
                 // x.IsGenericType: Arayüzün jenerik olup olmadığını kontrol eder.
                 // x.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>): Arayüzün jenerik  tanımının
                 // tam olarak IIntegrationEventHandler<> olup olmadığını kontrol eder.
                 // Bu adımın sonucu: Sadece IIntegrationEventHandler<OrderCreatedEvent>,  IIntegrationEventHandler<ProductUpdatedEvent> gibi arayüzler kalır.
                 .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>))
                 // 4. Adım: Her bir IIntegrationEventHandler<TEvent> arayüzünden, jenerik argümanı olan TEvent  tipini(yani işlenen olay tipini) al.
                 // inheritsType.GetGenericArguments().First(): Jenerik arayüzün jenerik argümanlarını (örn: <OrderCreatedEvent>) alır ve ilkini seçer.
                 // Bu adımın sonucu: OrderCreatedEvent, ProductUpdatedEvent gibi olay tipleri.
                 .Select(inheritsType => inheritsType.GetGenericArguments().First())
                 // 5. Adım: Elde edilen olay tiplerinin gerçekten IIntegrationEvent arayüzünü uygulayıp  uygulamadığını doğrula.
                 // Bu bir güvenlik kontrolüdür, çünkü IIntegrationEventHandler<> sadece IIntegrationEvent'ten  türeyen tipleri işleyebilir.
                 .Where(messageType => messageType.IsAssignableTo(typeof(IIntegrationEvent)))
                // 6. Adım: Tekrarlayan olay tiplerini ele.
                // Eğer birden fazla handler aynı olay tipini işliyorsa (örn: iki farklı handler   OrderCreatedEvent'i işliyorsa),
                // sadece bir kez listelenmesini sağlar.
                .Distinct();
        }



        public static bool IsEvent(this Type type)
            => type.IsAssignableTo(typeof(IEvent));
    }
}
