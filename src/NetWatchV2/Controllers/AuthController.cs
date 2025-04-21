using Microsoft.AspNetCore.Mvc;
using NetWatchV2.Data;
using NetWatchV2.Models;
using NetWatchV2.ViewModels;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Threading.Tasks;


namespace NetWatchV2.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Método para iniciar sesión.
        /// </summary>
        [HttpPost]
        public IActionResult Login(string correo, string contrasena, string ReturnUrl = null)
        {
            ViewData["ReturnUrl"] = ReturnUrl;
            if (ModelState.IsValid)
            {
                var usuario = _context.Usuarios.SingleOrDefault(u => u.Correo == correo);
                if (usuario != null)
                {
                    if (VerifyPassword(contrasena, usuario.Contrasena))
                    {
                        HttpContext.Session.SetInt32("UsuarioId", usuario.Id);

                        if (string.IsNullOrEmpty(ReturnUrl))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            return LocalRedirect(ReturnUrl);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Correo electrónico o contraseña incorrectos.");
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Método para registrar un nuevo usuario.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Usuarios.SingleOrDefault(u => u.Correo == model.Correo);
                if (existingUser == null)
                {
                    string salt = GenerateSalt();
                    string hashedPassword = HashPassword(model.Password, salt);

                    TimeZoneInfo costaRicaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");

                    var newUser = new Usuario
                    {
                        Nombre = model.Nombre,
                        Correo = model.Correo,
                        Contrasena = hashedPassword,
                        FechaRegistro = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, costaRicaTimeZone),
                        EsAdmin = false
                    };

                    _context.Usuarios.Add(newUser);
                    await _context.SaveChangesAsync();

                    ViewBag.Message = "Registro exitoso. Ahora puedes iniciar sesión.";
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("Correo", "Este correo electrónico ya está registrado.");
                }
            }
            return View(model);
        }

        /// <summary>
        /// Genera un salt aleatorio para el hash de la contraseña.
        /// </summary>
        /// <returns></returns>
        private string GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        /// <summary>
        /// Genera un hash de la contraseña.
        /// </summary>
        private string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            using (var rfc = new Rfc2898DeriveBytes(password, saltBytes, 10000))
            {
                byte[] hashBytes = rfc.GetBytes(20);
                byte[] hashWithSaltBytes = new byte[saltBytes.Length + hashBytes.Length];
                Array.Copy(saltBytes, hashWithSaltBytes, saltBytes.Length);
                Array.Copy(hashBytes, 0, hashWithSaltBytes, saltBytes.Length, hashBytes.Length);
                return Convert.ToBase64String(hashWithSaltBytes);
            }
        }

        /// <summary>
        /// Verifica si la contraseña ingresada coincide con el hash almacenado.
        /// </summary>
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            byte[] hashWithSaltBytes = Convert.FromBase64String(storedHash);
            byte[] saltBytes = new byte[16]; // La longitud del salt que generamos
            Array.Copy(hashWithSaltBytes, saltBytes, 16);
            string computedHash = HashPassword(enteredPassword, Convert.ToBase64String(saltBytes));
            return hashWithSaltBytes.SequenceEqual(Convert.FromBase64String(computedHash));
        }

        /// <summary>
        /// Método para cerrar sesión.
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login", "Auth");
        }
    }
}