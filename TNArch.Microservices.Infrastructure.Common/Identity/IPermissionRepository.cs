namespace TNArch.Microservices.Infrastructure.Common.Identity
{
    public interface IPermissionRepository
    {
        Task<string[]> GetPermissionsByRoles(string[] roles);
    }
}