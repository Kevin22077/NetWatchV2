using Microsoft.AspNetCore.Authorization;

namespace NetWatchV2.Auth
{
    public class AdminRequirement : IAuthorizationRequirement
    {
        public AdminRequirement() { }
    }
}