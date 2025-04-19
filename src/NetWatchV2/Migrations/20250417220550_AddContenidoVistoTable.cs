using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWatchV2.Migrations
{
    /// <inheritdoc />
    public partial class AddContenidoVistoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContenidosVistos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ContenidoId = table.Column<int>(type: "int", nullable: false),
                    FechaVisto = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContenidosVistos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContenidosVistos_Contenidos_ContenidoId",
                        column: x => x.ContenidoId,
                        principalTable: "Contenidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContenidosVistos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListasReproduccion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ContenidoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasReproduccion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasReproduccion_Contenidos_ContenidoId",
                        column: x => x.ContenidoId,
                        principalTable: "Contenidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListasReproduccion_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContenidosVistos_ContenidoId",
                table: "ContenidosVistos",
                column: "ContenidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ContenidosVistos_UsuarioId",
                table: "ContenidosVistos",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasReproduccion_ContenidoId",
                table: "ListasReproduccion",
                column: "ContenidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasReproduccion_UsuarioId",
                table: "ListasReproduccion",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContenidosVistos");

            migrationBuilder.DropTable(
                name: "ListasReproduccion");
        }
    }
}
