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

    public IActionResult Index()
    {
        int? usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        List<Contenido> recomendaciones = new List<Contenido>();
        List<Contenido> listaReproduccion = new List<Contenido>();

        if (usuarioId.HasValue)
        {
            // Lógica de recomendaciones (ejemplo básico: los últimos 5 contenidos agregados)
            recomendaciones = _context.Contenidos
                .OrderByDescending(c => c.Id)
                .Take(3)
                .ToList();

            // Obtener la lista de reproducción del usuario
            listaReproduccion = _context.ListasReproduccion
                .Where(lr => lr.UsuarioId == usuarioId.Value)
                .Include(lr => lr.Contenido)
                .Select(lr => lr.Contenido)
                .ToList();
        }
        else
        {
            // Si el usuario no ha iniciado sesión, mostrar algunas recomendaciones genéricas
            recomendaciones = _context.Contenidos
                .OrderBy(c => c.Año) // Ejemplo: ordenar por año para mostrar variedad
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
