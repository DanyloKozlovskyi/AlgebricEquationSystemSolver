using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlgebricEquationSystemSolver.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRoots__ : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("239f25ad-6f48-4c01-afb9-66e39313c534"),
                columns: new[] { "Parameters", "Roots" },
                values: new object[] { "[15,1,2,3,-1]", "[-1.59,3.85]" });

            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("ec1a1b75-11d5-4ec6-9d52-3ddae2eac040"),
                columns: new[] { "Parameters", "Roots" },
                values: new object[] { "[-90,1,-10,-100,9,1]", "[-15.43,6.55,-0.98]" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("239f25ad-6f48-4c01-afb9-66e39313c534"),
                columns: new[] { "Parameters", "Roots" },
                values: new object[] { "[10,20,30,40,50,60,72,84,95,10,101,102]", null });

            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("ec1a1b75-11d5-4ec6-9d52-3ddae2eac040"),
                columns: new[] { "Parameters", "Roots" },
                values: new object[] { "[1,2,3,4,5,6,7,8,9,10,11,12]", null });
        }
    }
}
