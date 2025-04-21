using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NetWatchV2.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NetWatchV2.Auth
{
    /// <summary>
    /// Handler de validacion para el requisito de administrador.
    /// </summary>
    public class AdminAuthorizationHandler : AuthorizationHandler<AdminRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public AdminAuthorizationHandler(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            var userIdClaim = context.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                var usuario = _context.Usuarios.FirstOrDefault(u => u.Id == userId && u.EsAdmin);
                if (usuario != null)
                {
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}