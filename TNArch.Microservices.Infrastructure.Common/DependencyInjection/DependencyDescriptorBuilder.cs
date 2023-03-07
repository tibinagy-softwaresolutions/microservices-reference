using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TNArch.Microservices.Core.Common.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.DependencyInjection
{
    public class DependencyDescriptorBuilder
    {
        public List<Action<Type, object[]>> ServiceFactories { get; } = new List<Action<Type, object[]>>();
        public IServiceCollection ServiceCollection { get; }
        public IConfiguration Configuration { get; set; }

        public DependencyDescriptorBuilder(IServiceCollection serviceCollcetion)
        {
            ServiceCollection = serviceCollcetion;
        }

        public DependencyDescriptorBuilder WithConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;

            return this;
        }

        public DependencyDescriptorBuilder ImplementsOpenGenericInterface(Type openGeneric)
        {
            ServiceFactories.Add((type, attributes) =>
            {
                var openGenericInterfaces = type.GetInterfaces().Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == openGeneric);

                openGenericInterfaces.ForEach(openGenericInterface => ServiceCollection.AddScoped(openGenericInterface, type));
            });

            return this;
        }

        public DependencyDescriptorBuilder ImplementsInterface<TInterface>()
        {
            ServiceFactories.Add((type, attributes) =>
            {
                var interfaces = type.GetInterfaces().Where(x => x == typeof(TInterface));

                interfaces.ForEach(openGenericInterface => ServiceCollection.AddScoped(openGenericInterface, type));
            });

            return this;
        }

        public DependencyDescriptorBuilder DecoratedWith<TAttribute>(DependencyGroups dependencyGroup = DependencyGroups.All) where TAttribute : DependencyAttribute
        {
            ServiceFactories.Add((type, attributes) =>
            {
                var serviceDescriptors = attributes.OfType<DependencyAttribute>().Where(a => a.GetType() == typeof(TAttribute) && a.DependencyGroup.HasFlag(dependencyGroup));

                serviceDescriptors.ForEach(serviceDescriptor => ServiceCollection.AddService(serviceDescriptor.ServiceType, type, serviceDescriptor.LifeStyle, serviceDescriptor));
            });

            return this;
        }

        public DependencyDescriptorBuilder DecoratedWithConfigurationDescriptor()
        {
            ServiceFactories.Add((type, attributes) =>
            {
                var configurationDescriptor = attributes.OfType<ConfigurationDescriptorAttribute>().FirstOrDefault();

                if (configurationDescriptor == null)
                    return;

                if (configurationDescriptor.IsCollection)
                    type = type.MakeArrayType();

                var instance = Configuration.GetSection(configurationDescriptor.ConfigurationPath ?? type.Name).Get(type);

                ServiceCollection.AddSingleton(type, instance);
            });

            return this;
        }
    }
}
