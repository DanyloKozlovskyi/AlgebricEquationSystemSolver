using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlgebricEquationSystemSolver.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCompletedColumnAlgebricSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Systems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("239f25ad-6f48-4c01-afb9-66e39313c534"),
                columns: new[] { "IsCompleted", "Parameters", "Roots" },
                values: new object[] { false, new List<double> { 15.0, 1.0, 2.0, 3.0, -1.0 }, new List<double> { -1.5900000000000001, 3.8500000000000001 } });

            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("ec1a1b75-11d5-4ec6-9d52-3ddae2eac040"),
                columns: new[] { "IsCompleted", "Parameters", "Roots" },
                values: new object[] { false, new List<double> { -90.0, 1.0, -10.0, -100.0, 9.0, 1.0 }, new List<double> { -15.43, 6.5499999999999998, -0.97999999999999998 } });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Systems");

            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("239f25ad-6f48-4c01-afb9-66e39313c534"),
                columns: new[] { "Parameters", "Roots" },
                values: new object[] { new List<double> { 15.0, 1.0, 2.0, 3.0, -1.0 }, new List<double> { -1.5900000000000001, 3.8500000000000001 } });

            migrationBuilder.UpdateData(
                table: "Systems",
                keyColumn: "Id",
                keyValue: new Guid("ec1a1b75-11d5-4ec6-9d52-3ddae2eac040"),
                columns: new[] { "Parameters", "Roots" },
                values: new object[] { new List<double> { -90.0, 1.0, -10.0, -100.0, 9.0, 1.0 }, new List<double> { -15.43, 6.5499999999999998, -0.97999999999999998 } });
        }
    }
}
