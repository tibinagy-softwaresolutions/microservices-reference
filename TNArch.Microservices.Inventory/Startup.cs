using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;
using TNArch.Microservices.Core.Common.Command;
using TNArch.Microservices.Core.Common.DependencyInjection;
using TNArch.Microservices.Infrastructure.Common.DependencyInjection;
using TNArch.Microservices.Infrastructure.Common.OpenApi;
using TNArch.Microservices.Inventory;

[assembly: WebJobsStartup(typeof(Startup))]
namespace TNArch.Microservices.Inventory
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.Configure<JsonSerializerOptions>(options =>
            {
                options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.PropertyNameCaseInsensitive = true;
                options.Converters.Add(new JsonStringEnumConverter());
            });

            builder.Services.AddConventionalDependencies("TNArch.Microservices",
                                                c => c.WithConfiguration(builder.GetContext().Configuration)
                                                      .ImplementsOpenGenericInterface(typeof(ICommandHandler<>))
                                                      .ImplementsOpenGenericInterface(typeof(ICommandHandler<,>))
                                                      .ImplementsOpenGenericInterface(typeof(IQueryHandler<,>))
                                                      .DecoratedWith<DependencyAttribute>()
                                                      .DecoratedWith<DecorateDependencyAttribute>()
                                                      .DecoratedWith<ReplaceDependencyAttribute>()
                                                      .DecoratedWithConfigurationDescriptor()
                                                      .UseCommandAsApiEndpoint("Inventory microservice"));
        }
    }
}
