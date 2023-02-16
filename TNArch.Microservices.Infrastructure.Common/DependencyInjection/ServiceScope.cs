
using TNArch.Microservices.Core.Common.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.DependencyInjection
{
    [Dependency(typeof(ServiceScope), DependencyLifeStyle.Scoped)]
    public class ServiceScope
    {
        public string Scope { get; set; }
    }
}
