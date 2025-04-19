using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NetWatchV2.Models;
using NetWatchV2.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;


namespace NetWatchV2.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Valida si el usuario se encuentra registrado y selecciona contenido de lista de reproduccion y recomendaciones.
    /// </summary>
    /// <returns> Lista de reproduccion y recomendaciones. </returns>
    public IActionResult Index()
    {
        int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        List<Contenido> recomendaciones = new List<Contenido>();
        List<Contenido> listaReproduccion = new List<Contenido>();

        if (usuarioId.HasValue)
        {
            listaReproduccion = _context.ListasReproduccion
                .Where(lr => lr.UsuarioId == usuarioId.Value)
                .Include(lr => lr.Contenido)
                .Select(lr => lr.Contenido)
                .ToList();

            recomendaciones = _context.Contenidos
                .OrderByDescending(c => c.Id)
                .Take(3)
                .ToList();
        }
        else
        {
            recomendaciones = _context.Contenidos
                .OrderBy(c => c.Año)
                .Take(5)
                .ToList();
        }

        ViewBag.Recomendaciones = recomendaciones;
        ViewBag.ListaReproduccion = listaReproduccion;

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
