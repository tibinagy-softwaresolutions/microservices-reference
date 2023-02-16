using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TNArch.Microservices.Core.Common.DependencyInjection;
using TNArch.Microservices.Infrastructure.Common.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceScope CreateScope(this IServiceProvider provider, string scope)
        {
            var serviceScope = provider.CreateScope();

            serviceScope.ServiceProvider.GetRequiredService<ServiceScope>().Scope = scope;

            return serviceScope;
        }

        public static void AddConventionalDependencies(this IServiceCollection collection, string namespaceToScan, params Action<DependencyDescriptorBuilder>[] builderConfigurators)
        {
            var types = Assembly.GetCallingAssembly().GetTypesFromAssemblyTree(namespaceToScan).Where(t => t.IsClass);

            foreach (var configure in builderConfigurators)
            {
                var dependencyBuilder = new DependencyDescriptorBuilder(collection);

                configure(dependencyBuilder);

                types.ForEach(t => dependencyBuilder.ServiceFactories.ForEach(sf => sf(t, t.GetCustomAttributes(false))));
            }
        }

        public static void AddService(this IServiceCollection services, Type serviceType, Type implementationType, DependencyLifeStyle lifeStyle, DependencyAttribute dependencyDescriptor)
        {
            var servicesByServiceType = services.Where(e => e.ServiceType == serviceType).ToArray();

            if (dependencyDescriptor is DecorateDependencyAttribute)
            {
                AddDecoratedService(services, serviceType, implementationType, lifeStyle, dependencyDescriptor, servicesByServiceType);
            }
            else if (dependencyDescriptor.ReplaceExistingService)
            {
                if (!servicesByServiceType.Any() && dependencyDescriptor.ServiceToReplace == null)
                {
                    services.Add(new ServiceDescriptor(serviceType, implementationType, (ServiceLifetime)lifeStyle));
                    return;
                }

                AddServiceReplacement(services, serviceType, implementationType, lifeStyle, dependencyDescriptor, servicesByServiceType);
            }
            else
            {
                var existingFactory = servicesByServiceType.OfType<ServiceFactoryDescriptor>().FirstOrDefault(s => s.ImplementationTypes.Contains(implementationType));

                if (existingFactory != null)
                    existingFactory.AddImplementation(implementationType);

                else if (dependencyDescriptor.Scope == null)
                    services.Add(new ServiceDescriptor(serviceType, implementationType, (ServiceLifetime)lifeStyle));

                else
                    services.Add(new ServiceFactoryDescriptor(new ServiceFactory(serviceType, implementationType), (ServiceLifetime)lifeStyle));
            }
        }

        private static void AddServiceReplacement(IServiceCollection services, Type serviceType, Type implementationType, DependencyLifeStyle lifeStyle, DependencyAttribute dependencyDescriptor, ServiceDescriptor[] servicesByServiceType)
        {
            var existingRegistration = servicesByServiceType.FirstOrDefault(e => e.GetType() == typeof(ServiceDescriptor) && (dependencyDescriptor.ServiceToReplace == null || e.ImplementationType == dependencyDescriptor.ServiceToReplace));
            var existingFactory = servicesByServiceType.OfType<ServiceFactoryDescriptor>().FirstOrDefault(e => dependencyDescriptor.ServiceToReplace == null || e.ImplementationTypes.Contains(dependencyDescriptor.ServiceToReplace));


            if (existingRegistration != null)
                services.Remove(existingRegistration);


            if (existingFactory != null)
                existingFactory.AddImplementation(implementationType);
            else
                services.Add(new ServiceFactoryDescriptor(new ServiceFactory(serviceType, implementationType, existingRegistration?.ImplementationType, dependencyDescriptor.ServiceToReplace), (ServiceLifetime)lifeStyle));
        }

        private static void AddDecoratedService(IServiceCollection services, Type serviceType, Type implementationType, DependencyLifeStyle lifeStyle, DependencyAttribute dependencyDescriptor, ServiceDescriptor[] servicesByServiceType)
        {
            var existingRegistration = servicesByServiceType.FirstOrDefault(e => e.ImplementationType == implementationType);

            if (existingRegistration != null)
                services.Remove(existingRegistration);

            var typeToDecorate = (dependencyDescriptor as DecorateDependencyAttribute).ServiceToReplace;

            var existingFactory = servicesByServiceType.OfType<ServiceFactoryDescriptor>().FirstOrDefault(e => e.ImplementationTypes.Contains(implementationType) || e.ImplementationTypes.Contains(typeToDecorate));

            if (existingFactory != null)
                existingFactory.AddImplementation(implementationType, typeToDecorate);
            else
                services.Add(new ServiceFactoryDescriptor(new ServiceFactory(serviceType, implementationType, typeToDecorate), (ServiceLifetime)lifeStyle));
        }

        public static void AddConfigurations(this IServiceCollection services, Action<IConfigurationBuilder> configurationBuilder)
        {
            var existingConfigurations = services
                 .Where(descriptor => descriptor.ServiceType == typeof(IConfiguration) && descriptor.ImplementationType is IConfigurationRoot)
                 .ToList();

            existingConfigurations.ForEach(cd => services.Remove(cd));

            var existingProviders = existingConfigurations.SelectMany(cd => (cd.ImplementationType as IConfigurationRoot).Providers);

            var builder = new ConfigurationBuilder();

            configurationBuilder(builder);

            var configurationRoot = new ConfigurationRoot(builder.Build().Providers.Concat(existingProviders).ToList());

            services.AddSingleton<IConfiguration>(configurationRoot);
        }

        private static IEnumerable<Type> GetTypesFromAssemblyTree(this Assembly assembly, string namespaceToScan)
        {
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith(namespaceToScan)).ToArray();

            var assembliesNotLoaded = Directory.GetFiles(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{namespaceToScan}*.dll")
                 .Select(AssemblyName.GetAssemblyName)
                 .Union(assembly.GetReferencedAssemblies().Where(a => a.FullName.StartsWith(namespaceToScan)))
                 .DistinctBy(a => a.FullName)
                 .Where(a => !loadedAssemblies.Any(la => la.FullName == a.FullName))
                 .ToArray();

            return loadedAssemblies
                .SelectMany(a => a.GetTypes())
                .Union(assembliesNotLoaded.SelectMany(ua => AppDomain.CurrentDomain.Load(ua).GetTypes())).ToArray();
        }
    }

}
