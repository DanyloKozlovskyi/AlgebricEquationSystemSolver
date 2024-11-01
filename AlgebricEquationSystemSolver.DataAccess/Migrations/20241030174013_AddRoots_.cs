using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlgebricEquationSystemSolver.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRoots_ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Roots",
                table: "Systems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("239f25ad-6f48-4c01-afb9-66e39313c534"),
                column: "Roots",
                value: null);

            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("ec1a1b75-11d5-4ec6-9d52-3ddae2eac040"),
                column: "Roots",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roots",
                table: "Systems");
        }
    }
}
