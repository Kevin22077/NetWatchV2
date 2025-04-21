using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetWatchV2.Data;
using NetWatchV2.ViewModels;
using System;
using System.Linq;

namespace NetWatchV2.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Método para mostrar el historial de contenido visto por el usuario.
        /// </summary>
        /// <returns></returns>
        public IActionResult ContenidoVisto()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (!usuarioId.HasValue)
            {
                TempData["Error"] = "Debes iniciar sesión para ver tu historial de contenido visto.";
                return RedirectToAction("Login", "Auth");
            }

            var contenidoVisto = _context.ContenidosVistos
                .Where(cv => cv.UsuarioId == usuarioId.Value)
                .Include(cv => cv.Contenido)
                .OrderByDescending(cv => cv.FechaVisto)
                .Select(cv => new ContenidoVistoViewModel
                {
                    NombreContenido = cv.Contenido.Nombre,
                    TipoContenido = cv.Contenido.Tipo,
                    FechaVisto = cv.FechaVisto
                })
                .ToList();

            return View(contenidoVisto);
        }

        /// <summary>
        /// Método para mostrar el tiempo total visto por el usuario.
        /// </summary>
        /// <returns></returns>
        public IActionResult TiempoTotalVisto()
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (!usuarioId.HasValue)
            {
                TempData["Error"] = "Debes iniciar sesión para ver el tiempo total invertido.";
                return RedirectToAction("Login", "Auth");
            }

            var contenidoVisto = _context.ContenidosVistos
                .Where(cv => cv.UsuarioId == usuarioId.Value)
                .Include(cv => cv.Contenido)
                .ToList();

            int totalMinutos = 0;
            foreach (var item in contenidoVisto)
            {
                if (!string.IsNullOrEmpty(item.Contenido.Duracion) && int.TryParse(item.Contenido.Duracion, out int minutos))
                {
                    totalMinutos += minutos;
                }
            }

            int horas = totalMinutos / 60;
            int minutosRestantes = totalMinutos % 60;

            ViewBag.TiempoTotal = $"{horas} horas y {minutosRestantes} minutos";
            return View();
        }
    }
}