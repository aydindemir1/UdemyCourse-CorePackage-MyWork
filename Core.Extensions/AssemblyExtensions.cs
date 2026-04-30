using System.Reflection;

namespace Core.Extensions
{
    public static class AssemblyExtensions
    {
        public static Assembly[]? GetDomainAssemblies(string pattern)
        {
            List<Assembly>? assemblies = new List<Assembly>();

            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var baazAssemblies = loadedAssemblies.Where(name => name.FullName!.Contains(pattern, StringComparison.CurrentCultureIgnoreCase)).ToArray();

            foreach (var assemblyName in baazAssemblies)
            {
                var assembly = Assembly.Load(assemblyName.FullName!);
                assemblies.Add(assembly);
            }
            return assemblies.ToArray();
        }

        //public static IServiceCollection AddSubClassesOfType(this IServiceCollection services, Assembly assembly, Type type, Func<IServiceCollection, Type, IServiceCollection>? addWithLifeCycle = null)
        //{
        //    var types = assembly.GetTypes().Where(t => t.IsSubclassOf(type) && type != t).ToList();
        //    foreach (Type? item in types)
        //    {
        //        if (addWithLifeCycle == null) { services.AddScoped(item); }
        //        else { addWithLifeCycle(services, item); }
        //    }

        //    return services;
        //}
    }
}
