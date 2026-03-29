using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusNow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ServiceAlerts_AffectedLineId",
                table: "ServiceAlerts",
                column: "AffectedLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAlerts_AffectedRouteId",
                table: "ServiceAlerts",
                column: "AffectedRouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAlerts_Routes_AffectedRouteId",
                table: "ServiceAlerts",
                column: "AffectedRouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAlerts_TransportLines_AffectedLineId",
                table: "ServiceAlerts",
                column: "AffectedLineId",
                principalTable: "TransportLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAlerts_Routes_AffectedRouteId",
                table: "ServiceAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAlerts_TransportLines_AffectedLineId",
                table: "ServiceAlerts");

            migrationBuilder.DropIndex(
                name: "IX_ServiceAlerts_AffectedLineId",
                table: "ServiceAlerts");

            migrationBuilder.DropIndex(
                name: "IX_ServiceAlerts_AffectedRouteId",
                table: "ServiceAlerts");
        }
    }
}
