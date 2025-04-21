using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWatchV2.Data;
using NetWatchV2.Models;
using System.Security.Cryptography;
using System;

namespace NetWatchV2.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Acción para mostrar la lista de usuarios.
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }

        /// <summary>
        /// Acción para mostrar el formulario de creación de un nuevo usuario.
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Acción para crear un nuevo usuario.
        /// </summary>
        /// <param name="nuevoUsuario"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(Usuario nuevoUsuario)
        {
            TimeZoneInfo costaRicaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            if (ModelState.IsValid)
            {
                string salt = GenerateSalt();
                string hashedPassword = HashPassword(nuevoUsuario.Contrasena, salt);
                nuevoUsuario.Contrasena = hashedPassword;
                nuevoUsuario.FechaRegistro = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, costaRicaTimeZone);
                _context.Usuarios.Add(nuevoUsuario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nuevoUsuario);
        }

        /// <summary>
        /// Accion para mostrar el formulario de eliminación de un usuario
        /// </summary>
        /// <param name="id"> el id del usuario a eliminar</param>
        /// <returns></returns>
        public IActionResult Delete(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        /// <summary>
        /// Acción para eliminar un usuario (solo para administradores).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
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
        /// Genera un hash de la contraseña utilizando PBKDF2 con un salt.
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
    }

}
