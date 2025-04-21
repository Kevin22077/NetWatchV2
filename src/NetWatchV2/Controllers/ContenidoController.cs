using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetWatchV2.Data;
using NetWatchV2.Models;
using System.Linq;

namespace NetWatchV2.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class ContenidoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContenidoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var contenidos = _context.Contenidos.ToList();
            return View(contenidos);
        }

        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Método para crear un nuevo contenido.
        /// </summary>
        /// <param name="contenido"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Create(Contenido contenido)
        {
            if (ModelState.IsValid)
            {
                contenido.FechaCreacion = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time"));
                _context.Contenidos.Add(contenido);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contenido);
        }


        public IActionResult Delete(int id)
        {
            var contenido = _context.Contenidos.Find(id);
            if (contenido == null)
            {
                return NotFound();
            }
            return View(contenido);
        }

        /// <summary>
        /// Método para eliminar un contenido existente.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var contenido = _context.Contenidos.Find(id);
            if (contenido != null)
            {
                _context.Contenidos.Remove(contenido);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var contenido = _context.Contenidos.Find(id);
            if (contenido == null)
            {
                return NotFound();
            }
            return View(contenido);
        }

        /// <summary>
        /// Método para editar un contenido existente.
        /// </summary>
        [HttpPost]
        public IActionResult Edit(Contenido contenido)
        {
            if (ModelState.IsValid)
            {
                _context.Update(contenido);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contenido);
        }

        /// <summary>
        /// Método para mostrar los detalles de un contenido específico.
        /// </summary>
        public IActionResult Details(int id)
        {
            var contenido = _context.Contenidos.Find(id);
            if (contenido == null)
            {
                return NotFound();
            }
            return View(contenido);
        }
    }
}