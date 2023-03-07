using System.Text.Json.Serialization;
using System.Text.Json;
using TNArch.Microservices.Infrastructure.Common.DependencyInjection;
using TNArch.Microservices.Core.Common.DependencyInjection;
using TNArch.Microservices.Core.Common.Command;
using TNArch.Microservices.Infrastructure.Common.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.PropertyNameCaseInsensitive = true;
    options.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConventionalDependencies("TNArch.Microservices",
                                    c => c.WithConfiguration(builder.Configuration)
                                          .ImplementsOpenGenericInterface(typeof(ICommandHandler<>))
                                          .ImplementsOpenGenericInterface(typeof(ICommandHandler<,>))
                                          .ImplementsOpenGenericInterface(typeof(IQueryHandler<,>))
                                          .ImplementsOpenGenericInterface(typeof(ICommandValidator<>))
                                          .DecoratedWith<DependencyAttribute>()
                                          .DecoratedWith<DecorateDependencyAttribute>()
                                          .DecoratedWith<ReplaceDependencyAttribute>()
                                          .DecoratedWithConfigurationDescriptor()
                                          .UseCommandAsApiEndpoint("Minimal Api Demo Microservice"));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.Map("/api/{command}", async (string command, ICommandDispatcherInvoker dispatcherInvocer, HttpRequest req) => Results.Json(await dispatcherInvocer.DispatchRequest(command, req)));

app.Run();
