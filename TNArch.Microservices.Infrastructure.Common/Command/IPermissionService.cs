using TNArch.Microservices.Core.Common.DependencyInjection;

namespace TNArch.Microservices.Core.Common.Command
{
    public interface IPermissionService
    {
        Task<bool> HasPermission(string permission);
    }

    [Dependency(typeof(IPermissionService))]
    public class PermissionService : IPermissionService
    {
        Task<bool> IPermissionService.HasPermission(string permission)
        {
            return Task.FromResult(true);
        }
    }
}