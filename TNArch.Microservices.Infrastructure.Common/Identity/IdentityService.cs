using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TNArch.Microservices.Core.Common.DependencyInjection;

namespace TNArch.Microservices.Infrastructure.Common.Identity
{
    [Dependency(typeof(IIdentityService))]
    public class IdentityService : IIdentityService
    {
        private JwtSecurityToken _token;
        private readonly IPermissionRepository _permissionRepository;

        public IdentityService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public void SetHttpContext(HttpRequest httpRequest)
        {
            _token = new JwtSecurityToken(httpRequest.Headers["Authorization"].First().Replace("Bearer ", ""));
        }

        public string GetUserName()
        {
            return _token.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        }

        public async Task<bool> HasPermission(string permission)
        {
            var roles = _token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToArray();

            var perimissionsByRoles = await _permissionRepository.GetPermissionsByRoles(roles);

            return perimissionsByRoles.Contains(permission);
        }
    }
}