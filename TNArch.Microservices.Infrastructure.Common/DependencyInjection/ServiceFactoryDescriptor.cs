using Microsoft.Extensions.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.DependencyInjection
{
    public class ServiceFactoryDescriptor : ServiceDescriptor
    {
        private readonly ServiceFactory _serviceFactory;

        public ServiceFactoryDescriptor(ServiceFactory serviceFactory, ServiceLifetime lifetime) : base(serviceFactory.GetServiceType(), serviceFactory.CreateInstance, lifetime)
        {
            _serviceFactory = serviceFactory;
        }

        public IEnumerable<Type> ImplementationTypes => _serviceFactory.GetIplementationTypes();

        public void AddImplementation(params Type[] implementations) => implementations.ForEach(i => _serviceFactory.AddImplementationType(i));
    }

}
