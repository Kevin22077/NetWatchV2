using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace NetWatchV2.Auth
{
    public class SessionAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IHttpContextAccessor httpContextAccessor)
            : base(options, logger, encoder, clock)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var sessionId = _httpContextAccessor.HttpContext.Session.GetInt32("UsuarioId");

            if (sessionId.HasValue)
            {
                // El usuario tiene un Id en la sesión, consideramos que está autenticado
                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, sessionId.ToString()),
                    new Claim(ClaimTypes.Name, sessionId.ToString()), // Puedes agregar más claims si es necesario
                };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }

            // No hay Id en la sesión, el usuario no está autenticado
            return AuthenticateResult.Fail("No user session found.");
        }
    }
}