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

        public IActionResult Index()
        {
            // Acción para listar todos los usuarios (solo para administradores)
            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }

        public IActionResult Create()
        {
            // Acción para mostrar el formulario de creación de un nuevo usuario (solo para administradores)
            return View();
        }

        [HttpPost]
        public IActionResult Create(Usuario nuevoUsuario)
        {
            TimeZoneInfo costaRicaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time");
            if (ModelState.IsValid)
            {
                string salt = GenerateSalt();
                string hashedPassword = HashPassword(nuevoUsuario.Contrasena, salt);
                nuevoUsuario.Contrasena = hashedPassword;
                nuevoUsuario.FechaRegistro = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, costaRicaTimeZone); // Establecer la fecha de registro
                _context.Usuarios.Add(nuevoUsuario);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nuevoUsuario);
        }

        public IActionResult Delete(int id)
        {
            // Acción para mostrar la confirmación de eliminación de un usuario (solo para administradores)
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Acción para eliminar realmente el usuario (solo para administradores)
            var usuario = _context.Usuarios.Find(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        private string GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }
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
