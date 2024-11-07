using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AlgebricEquationSystemSolver.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class added_tasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskCalculations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCalculations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCalculations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskCalculations_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CancellationTokenCalculations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    SystemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsCanceled = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CancellationTokenCalculations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CancellationTokenCalculations_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CancellationTokenCalculations_TaskCalculations_TaskId",
                        column: x => x.TaskId,
                        principalTable: "TaskCalculations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_CancellationTokenCalculations_SystemId",
                table: "CancellationTokenCalculations",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_CancellationTokenCalculations_TaskId",
                table: "CancellationTokenCalculations",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCalculations_SystemId",
                table: "TaskCalculations",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCalculations_UserId",
                table: "TaskCalculations",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CancellationTokenCalculations");

            migrationBuilder.DropTable(
                name: "TaskCalculations");

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
