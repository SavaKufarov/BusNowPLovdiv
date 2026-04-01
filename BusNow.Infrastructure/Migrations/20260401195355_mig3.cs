using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentStopOrderIndex",
                table: "Vehicles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RouteId",
                table: "Vehicles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_RouteId",
                table: "Vehicles",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Routes_RouteId",
                table: "Vehicles",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Routes_RouteId",
                table: "Vehicles");

            migrationBuilder.DropIndex(
                name: "IX_Vehicles_RouteId",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "CurrentStopOrderIndex",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "RouteId",
                table: "Vehicles");
        }
    }
}
