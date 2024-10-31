using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlgebricEquationSystemSolver.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class dummy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Equations",
                table: "Equations");

            migrationBuilder.RenameTable(
                name: "Equations",
                newName: "Systems");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Systems",
                table: "Systems",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Systems",
                table: "Systems");

            migrationBuilder.RenameTable(
                name: "Systems",
                newName: "Equations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Equations",
                table: "Equations",
                column: "Id");
        }
    }
}
