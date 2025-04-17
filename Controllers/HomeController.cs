using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using NetWatchV2.Models;
using NetWatchV2.Data;
using Microsoft.AspNetCore.Authorization;


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
        if (usuarioId.HasValue)
        {
            var usuarioAutenticado = _context.Usuarios.Find(usuarioId.Value);
            ViewBag.NombreUsuario = usuarioAutenticado?.Nombre;
        }
        else
        {
            ViewBag.NombreUsuario = "Invitado";
        }
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
