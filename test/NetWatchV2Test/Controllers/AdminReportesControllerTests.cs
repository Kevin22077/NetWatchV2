using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NetWatchV2.Controllers;
using NetWatchV2.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using NetWatchV2.Models;
using System.Collections.Generic;
using NetWatchV2Test.Helpers;
using iText.Commons.Actions.Contexts;

namespace NetWatchV2.Tests.Controllers
{
    [TestClass]
    public class AdminReportesControllerTests
    {
        private Mock<ApplicationDbContext> _mockContext;
        private AdminReportesController _controller;
        private DbContextOptions<ApplicationDbContext> _options;

        [TestInitialize]
        public void Setup()
        {
            // Configurar las opciones para la base de datos en memoria
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Usar una base de datos en memoria única para cada prueba
                .Options;

            // Crear una instancia del mock del contexto usando las opciones configuradas
            _mockContext = new Mock<ApplicationDbContext>(_options);

            _controller = new AdminReportesController(_mockContext.Object);
        }

        [TestMethod]
        public void Index_ReturnsDefaultView()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.ViewName);
        }

        [TestMethod]
        public async Task GenerarReporte_ReturnsPartialViewMostrarReporteWithCorrectViewBagData()
        {
            // Arrange
            int testMes = 7;
            int testAño = 2026;
            string expectedNombreMes = new DateTime(testAño, testMes, 1).ToString("MMMM", new CultureInfo("es-ES"));

            // Act
            var result = await _controller.GenerarReporte(testMes, testAño) as PartialViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("_MostrarReporte", result.ViewName);
            Assert.IsNotNull(_controller.ViewBag.MesSeleccionado);
            Assert.AreEqual(testMes, _controller.ViewBag.MesSeleccionado);
            Assert.IsNotNull(_controller.ViewBag.AñoSeleccionado);
            Assert.AreEqual(testAño, _controller.ViewBag.AñoSeleccionado);
            Assert.IsNotNull(_controller.ViewBag.NombreMesSeleccionado);
            Assert.AreEqual(expectedNombreMes, _controller.ViewBag.NombreMesSeleccionado);
        }

        [TestMethod]
        public async Task TotalUsuariosRegistrados_ReturnsPartialViewWithCorrectData()
        {
            // Arrange
            int testMes = 3;
            int testAño = 2024;
            int expectedTotalUsuarios = 2;

            // Simulacion de datos en la base de datos en memoria
            using (var context = new ApplicationDbContext(_options))
            {
                context.Usuarios.AddRange(new List<Usuario>
                {
                    new Usuario { FechaRegistro = new DateTime(2024, 3, 10), Nombre = "Usuario1", Correo = "usuario1@example.com", Contrasena = "password" },
                    new Usuario { FechaRegistro = new DateTime(2024, 3, 20), Nombre = "Usuario2", Correo = "usuario2@example.com", Contrasena = "password" },
                    new Usuario { FechaRegistro = new DateTime(2024, 4, 15), Nombre = "Usuario3", Correo = "usuario3@example.com", Contrasena = "password" }
                });
                await context.SaveChangesAsync();
            }

            //Instancia del controlador con el contexto en memoria
            using (var context = new ApplicationDbContext(_options))
            {
                _controller = new AdminReportesController(context);

                // Act
                var result = await _controller.TotalUsuariosRegistrados(testMes, testAño) as PartialViewResult;

                // Assert
                Assert.IsNotNull(result);
                Assert.AreEqual("_ReporteResultado", result.ViewName);
                Assert.IsNotNull(_controller.ViewBag.TotalUsuarios);
                Assert.AreEqual(expectedTotalUsuarios, _controller.ViewBag.TotalUsuarios);
                Assert.IsNotNull(_controller.ViewBag.ReporteTitulo);
                Assert.AreEqual($"Total de Usuarios Registrados en marzo 2024", _controller.ViewBag.ReporteTitulo);
                Assert.AreEqual(testMes, _controller.ViewBag.MesSeleccionado);
                Assert.AreEqual(testAño, _controller.ViewBag.AñoSeleccionado);
            }
        }

        
    }

}
