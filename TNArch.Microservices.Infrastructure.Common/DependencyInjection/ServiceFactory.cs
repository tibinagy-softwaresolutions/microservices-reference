using Microsoft.Extensions.DependencyInjection;
using TNArch.Microservices.Core.Common.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.DependencyInjection
{
    public class ServiceFactory
    {
        private readonly Type _serviceType;
        private readonly List<TypeRegistration> _implementationTypes;

        public ServiceFactory(Type serviceType, params Type[] implementationTypes)
        {
            _serviceType = serviceType;
            _implementationTypes = new List<TypeRegistration>();

            implementationTypes.ForEach(it => AddImplementationType(it));
        }

        internal IEnumerable<Type> GetIplementationTypes() => _implementationTypes.Select(i => i.ImplementationType);

        public Type GetServiceType() => _serviceType;

        public void AddImplementationType(Type implementationType)
        {
            if (implementationType == null || _implementationTypes.Any(t => t.ImplementationType == implementationType))
                return;

            var scopes = implementationType
                .GetCustomAttributes(true)
                .OfType<DependencyAttribute>()
                .Where(a => a.ServiceType == _serviceType)
                .Select(a => a.Scope)
                .ToArray();

            var decoratedObjects = implementationType
                .GetCustomAttributes(true)
                .OfType<DecorateDependencyAttribute>()
                .Where(a => a.ServiceType == _serviceType)
                .Select(a => new DecoratorTypeRegistration { ImplementationType = a.ServiceToReplace, ServiceScope = a.Scope })
                .ToArray();

            _implementationTypes.Add(new TypeRegistration { ImplementationType = implementationType, ServiceScopes = scopes, DecoratedServices = decoratedObjects });
        }

        public object CreateInstance(IServiceProvider serviceProvider)
        {
            var scopeFilter = serviceProvider.GetService<ServiceScope>().Scope;

            var implementations = _implementationTypes.Where(it => it.ServiceScopes.Contains(scopeFilter) || it.ServiceScopes.Contains(null)).ToArray();

            return CreateInstance(serviceProvider, scopeFilter, implementations);
        }

        private object CreateInstance(IServiceProvider serviceProvider, string scopeFilter, TypeRegistration[] implementationTypes)
        {
            var rootImplementations = implementationTypes
                .Where(i => !implementationTypes.Any(ii => ii.DecoratedServices.Any(ds => ds.ImplementationType == i.ImplementationType && (ds.ServiceScope == scopeFilter || ds.ServiceScope == null))));

            if (!rootImplementations.Any())
                return null;

            var rootImplementationByScope = rootImplementations.OrderBy(s => s.ServiceScopes.Contains(scopeFilter)).LastOrDefault();

            var decoratedServices = rootImplementationByScope.DecoratedServices
                .Where(ds => ds.ServiceScope == scopeFilter || ds.ServiceScope == null);

            if (!decoratedServices.Any())
                return ActivatorUtilities.GetServiceOrCreateInstance(serviceProvider, rootImplementationByScope.ImplementationType);

            implementationTypes = implementationTypes.Except(rootImplementations).ToArray();

            return ActivatorUtilities.CreateInstance(serviceProvider, rootImplementationByScope.ImplementationType, decoratedServices.Select(ds => CreateInstance(serviceProvider, scopeFilter, implementationTypes)).ToArray());
        }

        private sealed class TypeRegistration
        {
            public Type ImplementationType { get; set; }
            public string[] ServiceScopes { get; set; }
            public DecoratorTypeRegistration[] DecoratedServices { get; set; }
        }

        private sealed class DecoratorTypeRegistration
        {
            public Type ImplementationType { get; set; }
            public string ServiceScope { get; set; }
        }
    }

}
