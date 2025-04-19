using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWatchV2.Migrations
{
    /// <inheritdoc />
    public partial class AddListaOpinionesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListasOpiniones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    ContenidoId = table.Column<int>(type: "int", nullable: false),
                    CalificacionOpinion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OpinionTexto = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasOpiniones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListasOpiniones_Contenidos_ContenidoId",
                        column: x => x.ContenidoId,
                        principalTable: "Contenidos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListasOpiniones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListasOpiniones_ContenidoId",
                table: "ListasOpiniones",
                column: "ContenidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ListasOpiniones_UsuarioId",
                table: "ListasOpiniones",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListasOpiniones");
        }
    }
}
