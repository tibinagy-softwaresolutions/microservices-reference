using Microsoft.AspNetCore.Http;

namespace TNArch.Microservices.Infrastructure.Common.Identity
{
    public interface IIdentityService
    {
        void SetHttpContext(HttpRequest httpRequest);
        string GetUserName();
        Task<bool> HasPermission(string permission);
    }
}