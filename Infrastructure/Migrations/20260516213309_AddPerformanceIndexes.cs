using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_SECTOR_EventId",
                table: "SECTOR",
                newName: "IX_Sector_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_SEAT_SectorId",
                table: "SEAT",
                newName: "IX_Seat_SectorId");

            migrationBuilder.RenameIndex(
                name: "IX_RESERVATION_SeatId",
                table: "RESERVATION",
                newName: "IX_Reservation_SeatId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SEAT",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "RESERVATION",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EntityId",
                table: "AUDIT_LOG",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Seat_Status",
                table: "SEAT",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_Status_ExpiresAt",
                table: "RESERVATION",
                columns: new[] { "Status", "ExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_EntityId",
                table: "AUDIT_LOG",
                column: "EntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seat_Status",
                table: "SEAT");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_Status_ExpiresAt",
                table: "RESERVATION");

            migrationBuilder.DropIndex(
                name: "IX_AuditLog_EntityId",
                table: "AUDIT_LOG");

            migrationBuilder.RenameIndex(
                name: "IX_Sector_EventId",
                table: "SECTOR",
                newName: "IX_SECTOR_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_Seat_SectorId",
                table: "SEAT",
                newName: "IX_SEAT_SectorId");

            migrationBuilder.RenameIndex(
                name: "IX_Reservation_SeatId",
                table: "RESERVATION",
                newName: "IX_RESERVATION_SeatId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "SEAT",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "RESERVATION",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "EntityId",
                table: "AUDIT_LOG",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
