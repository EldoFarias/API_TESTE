using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WFConFin.Migrations
{
    /// <inheritdoc />
    public partial class MigrarNomeColuna1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Genereo",
                table: "Pessoa",
                newName: "Genero");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Genero",
                table: "Pessoa",
                newName: "Genereo");
        }
    }
}
