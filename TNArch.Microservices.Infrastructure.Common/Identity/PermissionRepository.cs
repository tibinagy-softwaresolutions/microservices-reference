using TNArch.Microservices.Core.Common.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.Identity
{
    [Dependency(typeof(IPermissionRepository))]
    public class PermissionRepository : IPermissionRepository
    {
        public Task<string[]> GetPermissionsByRoles(string[] roles)
        {
            return Task.FromResult(new string[] { "Demo.Items.Read" });
        }
    }
}