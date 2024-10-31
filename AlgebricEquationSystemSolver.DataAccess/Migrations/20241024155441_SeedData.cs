using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlgebricEquationSystemSolver.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Systems",
                columns: new[] { "Id", "Parameters" },
                values: new object[,]
                {
                    { new Guid("239f25ad-6f48-4c01-afb9-66e39313c534"), "[10,20,30,40,50,60,72,84,95,10,101,102]" },
                    { new Guid("ec1a1b75-11d5-4ec6-9d52-3ddae2eac040"), "[1,2,3,4,5,6,7,8,9,10,11,12]" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("239f25ad-6f48-4c01-afb9-66e39313c534"));

            migrationBuilder.DeleteData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("ec1a1b75-11d5-4ec6-9d52-3ddae2eac040"));
        }
    }
}
