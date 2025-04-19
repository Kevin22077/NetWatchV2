using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetWatchV2.Data;
using System.Globalization;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;


namespace NetWatchV2.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AdminReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerarReporte(int mes, int año)
        {
            ViewBag.MesSeleccionado = mes;
            ViewBag.AñoSeleccionado = año;
            ViewBag.NombreMesSeleccionado = new DateTime(año, mes, 1).ToString("MMMM", new CultureInfo("es-ES"));
            return PartialView("_MostrarReporte");
        }

        public async Task<IActionResult> TotalUsuariosRegistrados(int mes, int año)
        {
            var totalUsuarios = await _context.Usuarios
                .Where(u => u.FechaRegistro.Year == año && u.FechaRegistro.Month == mes)
                .CountAsync();
            ViewBag.TotalUsuarios = totalUsuarios;
            ViewBag.ReporteTitulo = $"Total de Usuarios Registrados en {new DateTime(año, mes, 1).ToString("MMMM yyyy", new CultureInfo("es-ES"))}";
            ViewBag.MesSeleccionado = mes;
            ViewBag.AñoSeleccionado = año;
            return PartialView("_ReporteResultado");
        }

        public async Task<IActionResult> NuevosUsuariosRegistrados(int mes, int año)
        {
            var nuevosUsuarios = await _context.Usuarios
                .Where(u => u.FechaRegistro.Year == año && u.FechaRegistro.Month == mes)
                .Select(u => new { u.Nombre, u.Correo, u.FechaRegistro })
                .ToListAsync();
            ViewBag.NuevosUsuarios = nuevosUsuarios;
            ViewBag.ReporteTitulo = $"Nuevos Usuarios Registrados en {new DateTime(año, mes, 1).ToString("MMMM yyyy", new CultureInfo("es-ES"))}";
            ViewBag.MesSeleccionado = mes;
            ViewBag.AñoSeleccionado = año;
            return PartialView("_ReporteResultado");
        }

        public async Task<IActionResult> ContenidoMasVisto(int mes, int año)
        {
            var contenidoVisto = await _context.ContenidosVistos
                .Where(cv => cv.FechaVisto.Year == año && cv.FechaVisto.Month == mes)
                .Include(cv => cv.Contenido)
                .GroupBy(cv => new { cv.ContenidoId, cv.Contenido.Nombre, cv.Contenido.Genero })
                .Select(g => new { ContenidoNombre = g.Key.Nombre, Genero = g.Key.Genero, VecesVisto = g.Count() })
                .OrderByDescending(g => g.VecesVisto)
                .ToListAsync();
            ViewBag.ContenidoMasVisto = contenidoVisto;
            ViewBag.ReporteTitulo = $"Contenido Más Visto en {new DateTime(año, mes, 1).ToString("MMMM yyyy", new CultureInfo("es-ES"))}";
            ViewBag.MesSeleccionado = mes;
            ViewBag.AñoSeleccionado = año;
            return PartialView("_ReporteResultado");
        }

        public async Task<IActionResult> UsuariosConActividad(int mes, int año)
        {
            var usuariosActivosVisto = await _context.ContenidosVistos
                .Where(cv => cv.FechaVisto.Year == año && cv.FechaVisto.Month == mes)
                .Include(cv => cv.Usuario)
                .Select(cv => new { cv.Usuario.Nombre, TipoActividad = "Visto", Fecha = cv.FechaVisto })
                .ToListAsync();

            var usuariosActivosOpinion = await _context.ListasOpiniones
                .Where(lo => lo.FechaCreacion.Year == año && lo.FechaCreacion.Month == mes)
                .Include(lo => lo.Usuario)
                .Select(lo => new { lo.Usuario.Nombre, TipoActividad = "Opinión", Fecha = lo.FechaCreacion })
                .ToListAsync();

            var usuariosActivos = usuariosActivosVisto.Concat(usuariosActivosOpinion)
                .OrderByDescending(ua => ua.Fecha)
                .ToList();

            ViewBag.UsuariosConActividad = usuariosActivos;
            ViewBag.ReporteTitulo = $"Usuarios con Actividad en {new DateTime(año, mes, 1).ToString("MMMM yyyy", new CultureInfo("es-ES"))}";
            ViewBag.MesSeleccionado = mes;
            ViewBag.AñoSeleccionado = año;
            return PartialView("_ReporteResultado");
        }

        // Métodos para descargar reportes
        public async Task<IActionResult> DescargarTotalUsuariosRegistrados(int mes, int año, string formato)
        {
            if (mes < 1 || mes > 12 || año < 1 || año > DateTime.MaxValue.Year)
            {
                return Content($"Error: Mes ({mes}) o Año ({año}) inválido para la descarga.");
            }

            var totalUsuarios = await _context.Usuarios
                .Where(u => u.FechaRegistro.Year == año && u.FechaRegistro.Month == mes)
                .CountAsync();

            var nombreArchivoBase = $"TotalUsuariosRegistrados_{año}_{mes}";

            if (formato.ToLower() == "csv")
            {
                var nombreArchivo = $"{nombreArchivoBase}.csv";
                var contenido = $"Total Usuarios Registrados,{totalUsuarios}\n";
                return File(System.Text.Encoding.UTF8.GetBytes(contenido), "text/csv", nombreArchivo);
            }
            else if (formato.ToLower() == "pdf")
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new PdfWriter(ms))
                    using (var pdfDocument = new PdfDocument(writer))
                    using (var document = new Document(pdfDocument))
                    {
                        document.Add(new Paragraph($"Reporte: Total de Usuarios Registrados"));
                        document.Add(new Paragraph($"Mes: {new DateTime(año, mes, 1).ToString("MMMM", new CultureInfo("es-ES"))}"));
                        document.Add(new Paragraph($"Año: {año}"));
                        document.Add(new Paragraph($"Total de Usuarios Registrados: {totalUsuarios}"));
                    }
                    var nombreArchivo = $"{nombreArchivoBase}.pdf";
                    return File(ms.ToArray(), "application/pdf", nombreArchivo);
                }
            }
            else
            {
                return Content("Formato de descarga no soportado.");
            }
        }

        public async Task<IActionResult> DescargarNuevosUsuariosRegistrados(int mes, int año, string formato)
        {
            if (mes < 1 || mes > 12 || año < 1 || año > DateTime.MaxValue.Year)
            {
                return Content($"Error: Mes ({mes}) o Año ({año}) inválido para la descarga.");
            }

            var nuevosUsuarios = await _context.Usuarios
                .Where(u => u.FechaRegistro.Year == año && u.FechaRegistro.Month == mes)
                .Select(u => new { u.Nombre, u.Correo, u.FechaRegistro })
                .ToListAsync();

            var nombreArchivoBase = $"NuevosUsuariosRegistrados_{año}_{mes}";

            if (formato.ToLower() == "csv")
            {
                var nombreArchivo = $"{nombreArchivoBase}.csv";
                var contenido = "Nombre,Email,Fecha de Registro\n";
                foreach (var usuario in nuevosUsuarios)
                {
                    contenido += $"{usuario.Nombre},{usuario.Correo},{usuario.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss")}\n";
                }
                return File(System.Text.Encoding.UTF8.GetBytes(contenido), "text/csv", nombreArchivo);
            }
            else if (formato.ToLower() == "pdf")
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new PdfWriter(ms))
                    using (var pdfDocument = new PdfDocument(writer))
                    using (var document = new Document(pdfDocument))
                    {
                        document.Add(new Paragraph($"Reporte: Nuevos Usuarios Registrados"));
                        document.Add(new Paragraph($"Mes: {new DateTime(año, mes, 1).ToString("MMMM", new CultureInfo("es-ES"))}"));
                        document.Add(new Paragraph($"Año: {año}"));

                        Table table = new Table(3);
                        table.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));
                        table.AddHeaderCell("Nombre");
                        table.AddHeaderCell("Email");
                        table.AddHeaderCell("Fecha de Registro");

                        foreach (var usuario in nuevosUsuarios)
                        {
                            table.AddCell(usuario.Nombre);
                            table.AddCell(usuario.Correo);
                            table.AddCell(usuario.FechaRegistro.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        document.Add(table);
                    }
                    var nombreArchivo = $"{nombreArchivoBase}.pdf";
                    return File(ms.ToArray(), "application/pdf", nombreArchivo);
                }
            }
            else
            {
                return Content("Formato de descarga no soportado.");
            }

        }

        public async Task<IActionResult> DescargarContenidoMasVisto(int mes, int año, string formato)
        {
            if (mes < 1 || mes > 12 || año < 1 || año > DateTime.MaxValue.Year)
            {
                return Content($"Error: Mes ({mes}) o Año ({año}) inválido para la descarga.");
            }

            var contenidoVisto = await _context.ContenidosVistos
                .Where(cv => cv.FechaVisto.Year == año && cv.FechaVisto.Month == mes)
                .Include(cv => cv.Contenido)
                .GroupBy(cv => new { cv.ContenidoId, cv.Contenido.Nombre, cv.Contenido.Genero })
                .Select(g => new { ContenidoNombre = g.Key.Nombre, Genero = g.Key.Genero, VecesVisto = g.Count() })
                .OrderByDescending(g => g.VecesVisto)
                .ToListAsync();

            var nombreArchivoBase = $"ContenidoMasVisto_{año}_{mes}";

            if (formato.ToLower() == "csv")
            {
                var nombreArchivo = $"{nombreArchivoBase}.csv";
                var contenido = "Contenido,Genero,Veces Visto\n";
                foreach (var item in contenidoVisto)
                {
                    contenido += $"{item.ContenidoNombre},{item.Genero},{item.VecesVisto}\n";
                }
                return File(System.Text.Encoding.UTF8.GetBytes(contenido), "text/csv", nombreArchivo);
            }
            else if (formato.ToLower() == "pdf")
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new PdfWriter(ms))
                    using (var pdfDocument = new PdfDocument(writer))
                    using (var document = new Document(pdfDocument))
                    {
                        document.Add(new Paragraph($"Reporte: Contenido Más Visto"));
                        document.Add(new Paragraph($"Mes: {new DateTime(año, mes, 1).ToString("MMMM", new CultureInfo("es-ES"))}"));
                        document.Add(new Paragraph($"Año: {año}"));

                        Table table = new Table(3);
                        table.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));
                        table.AddHeaderCell("Contenido");
                        table.AddHeaderCell("Género");
                        table.AddHeaderCell("Veces Visto");

                        foreach (var item in contenidoVisto)
                        {
                            table.AddCell(item.ContenidoNombre);
                            table.AddCell(item.Genero);
                            table.AddCell(item.VecesVisto.ToString());
                        }
                        document.Add(table);
                    }
                    var nombreArchivo = $"{nombreArchivoBase}.pdf";
                    return File(ms.ToArray(), "application/pdf", nombreArchivo);
                }
            }
            else
            {
                return Content("Formato de descarga no soportado.");
            }
        }

        public async Task<IActionResult> DescargarUsuariosConActividad(int mes, int año, string formato)
        {
            if (mes < 1 || mes > 12 || año < 1 || año > DateTime.MaxValue.Year)
            {
                return Content($"Error: Mes ({mes}) o Año ({año}) inválido para la descarga.");
            }

            var usuariosActivosVisto = await _context.ContenidosVistos
                .Where(cv => cv.FechaVisto.Year == año && cv.FechaVisto.Month == mes)
                .Include(cv => cv.Usuario)
                .Select(cv => new { cv.Usuario.Nombre, TipoActividad = "Visto", Fecha = cv.FechaVisto })
                .ToListAsync();

            var usuariosActivosOpinion = await _context.ListasOpiniones
                .Where(lo => lo.FechaCreacion.Year == año && lo.FechaCreacion.Month == mes)
                .Include(lo => lo.Usuario)
                .Select(lo => new { lo.Usuario.Nombre, TipoActividad = "Opinión", Fecha = lo.FechaCreacion })
                .ToListAsync();

            var usuariosActivos = usuariosActivosVisto.Concat(usuariosActivosOpinion)
                .OrderByDescending(ua => ua.Fecha)
                .ToList();

            var nombreArchivoBase = $"UsuariosConActividad_{año}_{mes}";

            if (formato.ToLower() == "csv")
            {
                var nombreArchivo = $"{nombreArchivoBase}.csv";
                var contenido = "Usuario,Tipo de Actividad,Fecha\n";
                foreach (var item in usuariosActivos)
                {
                    contenido += $"{item.Nombre},{item.TipoActividad},{item.Fecha.ToString("yyyy-MM-dd HH:mm:ss")}\n";
                }
                return File(System.Text.Encoding.UTF8.GetBytes(contenido), "text/csv", nombreArchivo);
            }
            else if (formato.ToLower() == "pdf")
            {
                using (var ms = new MemoryStream())
                {
                    using (var writer = new PdfWriter(ms))
                    using (var pdfDocument = new PdfDocument(writer))
                    using (var document = new Document(pdfDocument))
                    {
                        document.Add(new Paragraph($"Reporte: Usuarios con Actividad"));
                        document.Add(new Paragraph($"Mes: {new DateTime(año, mes, 1).ToString("MMMM", new CultureInfo("es-ES"))}"));
                        document.Add(new Paragraph($"Año: {año}"));

                        Table table = new Table(3);
                        table.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));
                        table.AddHeaderCell("Usuario");
                        table.AddHeaderCell("Tipo de Actividad");
                        table.AddHeaderCell("Fecha");

                        foreach (var item in usuariosActivos)
                        {
                            table.AddCell(item.Nombre);
                            table.AddCell(item.TipoActividad);
                            table.AddCell(item.Fecha.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        document.Add(table);
                    }
                    var nombreArchivo = $"{nombreArchivoBase}.pdf";
                    return File(ms.ToArray(), "application/pdf", nombreArchivo);
                }
            }
            else
            {
                return Content("Formato de descarga no soportado.");
            }
        }
    }
}