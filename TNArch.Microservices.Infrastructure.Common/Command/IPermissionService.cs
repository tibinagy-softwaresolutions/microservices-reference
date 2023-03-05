using Microsoft.AspNetCore.Http;
using System.Security.Claims;
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
        private readonly IIdentityService _identityService;

        public PermissionService(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        Task<bool> IPermissionService.HasPermission(string permission)
        {
            return Task.FromResult(true);
        }
    }

    public interface IIdentityService
    {
        public string GetUserName();
        public string[] GetRoles();
        public string[] GetPermissions();
    }

    [Dependency(typeof(IIdentityService))]
    public class IdentityService : IIdentityService
    {
        private readonly HttpRequest _httpRequest;

        public IdentityService(HttpRequest httpRequest)
        {
            _httpRequest = httpRequest;
        }

        public string[] GetPermissions()
        {
            throw new NotImplementedException();
        }

        public string[] GetRoles()
        {
            throw new NotImplementedException();
        }

        public string GetUserName()
        {
            throw new NotImplementedException();
        }
    }

}