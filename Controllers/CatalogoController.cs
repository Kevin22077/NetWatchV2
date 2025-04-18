using Microsoft.AspNetCore.Mvc;
using NetWatchV2.Data;
using NetWatchV2.Models;
using NetWatchV2.ViewModels;
using System.Linq;

namespace NetWatchV2.Controllers
{
    public class CatalogoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CatalogoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string nombreFiltro, string generoFiltro, string plataformaFiltro, string calificacionFiltro)
        {
            var contenidos = _context.Contenidos.AsQueryable();

            if (!string.IsNullOrEmpty(nombreFiltro))
            {
                contenidos = contenidos.Where(c => c.Nombre.Contains(nombreFiltro));
            }

            if (!string.IsNullOrEmpty(generoFiltro))
            {
                contenidos = contenidos.Where(c => c.Genero.Contains(generoFiltro));
            }

            if (!string.IsNullOrEmpty(plataformaFiltro))
            {
                contenidos = contenidos.Where(c => c.Plataforma == plataformaFiltro);
            }

            if (!string.IsNullOrEmpty(calificacionFiltro))
            {
                if (calificacionFiltro == "1-2") contenidos = contenidos.Where(c => c.Calificacion >= 1 && c.Calificacion < 2);
                if (calificacionFiltro == "2-3") contenidos = contenidos.Where(c => c.Calificacion >= 2 && c.Calificacion < 3);
                if (calificacionFiltro == "3-4") contenidos = contenidos.Where(c => c.Calificacion >= 3 && c.Calificacion < 4);
                if (calificacionFiltro == "4-5") contenidos = contenidos.Where(c => c.Calificacion >= 4 && c.Calificacion < 5);
                if (calificacionFiltro == "5-6") contenidos = contenidos.Where(c => c.Calificacion >= 5 && c.Calificacion < 6);
                if (calificacionFiltro == "6-7") contenidos = contenidos.Where(c => c.Calificacion >= 6 && c.Calificacion < 7);
                if (calificacionFiltro == "7-8") contenidos = contenidos.Where(c => c.Calificacion >= 7 && c.Calificacion < 8);
                if (calificacionFiltro == "8-9") contenidos = contenidos.Where(c => c.Calificacion >= 8 && c.Calificacion < 9);
                if (calificacionFiltro == "9-10") contenidos = contenidos.Where(c => c.Calificacion >= 9 && c.Calificacion <= 10);
            }

            return View(contenidos.ToList());
        }

        public IActionResult Details(int id)
        {
            var contenido = _context.Contenidos.Find(id);
            if (contenido == null)
            {
                return NotFound();
            }

            var opiniones = _context.ListasOpiniones
                .Where(o => o.ContenidoId == id)
                .Select(o => new OpinionViewModel
                {
                    NombreUsuario = o.Usuario.Nombre,
                    Calificacion = o.CalificacionOpinion,
                    TextoOpinion = o.OpinionTexto
                })
                .ToList();

            ViewBag.Opiniones = opiniones;

            return View(contenido);
        }

        [HttpPost]
        public IActionResult MarcarVisto(int contenidoId)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId.HasValue)
            {
                var existente = _context.ContenidosVistos.FirstOrDefault(cv => cv.UsuarioId == usuarioId.Value && cv.ContenidoId == contenidoId);
                if (existente == null)
                {
                    var contenidoVisto = new ContenidoVisto
                    {
                        UsuarioId = usuarioId.Value,
                        ContenidoId = contenidoId,
                        FechaVisto = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time")) // Hora de Costa Rica
                    };
                    _context.ContenidosVistos.Add(contenidoVisto);
                    _context.SaveChanges();
                    TempData["Mensaje"] = "Contenido marcado como visto.";
                }
                else
                {
                    TempData["Mensaje"] = "Este contenido ya fue marcado como visto.";
                }
            }
            else
            {
                TempData["Error"] = "Debes iniciar sesión para marcar contenido como visto.";
            }
            return RedirectToAction("Details", new { id = contenidoId });
        }

        [HttpPost]
        public IActionResult AgregarListaReproduccion(int contenidoId)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId.HasValue)
            {
                var existente = _context.ListasReproduccion.FirstOrDefault(lr => lr.UsuarioId == usuarioId.Value && lr.ContenidoId == contenidoId);
                if (existente == null)
                {
                    var listaReproduccion = new ListaReproduccion
                    {
                        UsuarioId = usuarioId.Value,
                        ContenidoId = contenidoId
                    };
                    _context.ListasReproduccion.Add(listaReproduccion);
                    _context.SaveChanges();
                    TempData["Mensaje"] = "Contenido agregado a la lista de reproducción.";
                }
                else
                {
                    TempData["Mensaje"] = "Este contenido ya está en tu lista de reproducción.";
                }
            }
            else
            {
                TempData["Error"] = "Debes iniciar sesión para agregar contenido a la lista de reproducción.";
            }
            return RedirectToAction("Details", new { id = contenidoId });
        }

        [HttpPost]
        public IActionResult GuardarOpinion(int contenidoId, string calificacionOpinion, string opinionTexto)
        {
            int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            if (usuarioId.HasValue)
            {
                if (!string.IsNullOrEmpty(calificacionOpinion))
                {
                    var nuevaOpinion = new ListaOpiniones
                    {
                        UsuarioId = usuarioId.Value,
                        ContenidoId = contenidoId,
                        CalificacionOpinion = calificacionOpinion,
                        OpinionTexto = opinionTexto
                    };
                    _context.ListasOpiniones.Add(nuevaOpinion);
                    _context.SaveChanges();
                    TempData["MensajeOpinion"] = "Tu opinión ha sido guardada.";
                }
                else
                {
                    TempData["ErrorOpinion"] = "Debes seleccionar una calificación para tu opinión.";
                }
            }
            else
            {
                TempData["ErrorOpinion"] = "Debes iniciar sesión para dejar una opinión.";
            }
            return RedirectToAction("Details", new { id = contenidoId });
        }
    }
}