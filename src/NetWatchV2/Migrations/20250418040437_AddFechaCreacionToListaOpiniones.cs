using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetWatchV2.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaCreacionToListaOpiniones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCreacion",
                table: "ListasOpiniones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "ListasOpiniones");
        }
    }
}
