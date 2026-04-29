using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EVENT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EventDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Venue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENT", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "USER",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_USER", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SECTOR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SECTOR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SECTOR_EVENT_EventId",
                        column: x => x.EventId,
                        principalTable: "EVENT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AUDIT_LOG",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AUDIT_LOG", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AUDIT_LOG_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "USER",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SEAT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectorId = table.Column<int>(type: "int", nullable: false),
                    RowIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SEAT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SEAT_SECTOR_SectorId",
                        column: x => x.SectorId,
                        principalTable: "SECTOR",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RESERVATION",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SeatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReservedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RESERVATION", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RESERVATION_SEAT_SeatId",
                        column: x => x.SeatId,
                        principalTable: "SEAT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RESERVATION_USER_UserId",
                        column: x => x.UserId,
                        principalTable: "USER",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EVENT",
                columns: new[] { "Id", "EventDate", "Name", "Status", "Venue" },
                values: new object[] { 1, new DateTime(2026, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Concierto de Rock", "Active", "Estadio Unaj" });

            migrationBuilder.InsertData(
                table: "USER",
                columns: new[] { "Id", "Email", "Name", "PasswordHash" },
                values: new object[] { 1, "test@example.com", "Usuario Test", "hashed_password" });

            migrationBuilder.InsertData(
                table: "SECTOR",
                columns: new[] { "Id", "Capacity", "EventId", "Name", "Price" },
                values: new object[,]
                {
                    { 1, 50, 1, "Platea Alta", 5000m },
                    { 2, 50, 1, "Campo General", 3000m }
                });

            migrationBuilder.InsertData(
                table: "SEAT",
                columns: new[] { "Id", "RowIdentifier", "SeatNumber", "SectorId", "Status", "Version" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "A", 1, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "A", 2, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "A", 3, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "A", 4, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "A", 5, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "A", 6, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "A", 7, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "A", 8, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "A", 9, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "A", 10, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "A", 11, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "A", 12, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "A", 13, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "A", 14, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "A", 15, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "A", 16, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000017"), "A", 17, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000018"), "A", 18, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000019"), "A", 19, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000020"), "A", 20, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000021"), "A", 21, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000022"), "A", 22, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000023"), "A", 23, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000024"), "A", 24, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000025"), "A", 25, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000026"), "A", 26, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000027"), "A", 27, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000028"), "A", 28, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000029"), "A", 29, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000030"), "A", 30, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000031"), "A", 31, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000032"), "A", 32, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000033"), "A", 33, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000034"), "A", 34, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000035"), "A", 35, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000036"), "A", 36, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000037"), "A", 37, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000038"), "A", 38, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000039"), "A", 39, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000040"), "A", 40, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000041"), "A", 41, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000042"), "A", 42, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000043"), "A", 43, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000044"), "A", 44, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000045"), "A", 45, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000046"), "A", 46, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000047"), "A", 47, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000048"), "A", 48, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000049"), "A", 49, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000050"), "A", 50, 1, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000101"), "B", 1, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000102"), "B", 2, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000103"), "B", 3, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000104"), "B", 4, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000105"), "B", 5, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000106"), "B", 6, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000107"), "B", 7, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000108"), "B", 8, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000109"), "B", 9, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000110"), "B", 10, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000111"), "B", 11, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000112"), "B", 12, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000113"), "B", 13, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000114"), "B", 14, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000115"), "B", 15, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000116"), "B", 16, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000117"), "B", 17, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000118"), "B", 18, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000119"), "B", 19, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000120"), "B", 20, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000121"), "B", 21, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000122"), "B", 22, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000123"), "B", 23, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000124"), "B", 24, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000125"), "B", 25, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000126"), "B", 26, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000127"), "B", 27, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000128"), "B", 28, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000129"), "B", 29, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000130"), "B", 30, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000131"), "B", 31, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000132"), "B", 32, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000133"), "B", 33, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000134"), "B", 34, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000135"), "B", 35, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000136"), "B", 36, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000137"), "B", 37, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000138"), "B", 38, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000139"), "B", 39, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000140"), "B", 40, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000141"), "B", 41, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000142"), "B", 42, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000143"), "B", 43, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000144"), "B", 44, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000145"), "B", 45, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000146"), "B", 46, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000147"), "B", 47, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000148"), "B", 48, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000149"), "B", 49, 2, "Available", 1 },
                    { new Guid("00000000-0000-0000-0000-000000000150"), "B", 50, 2, "Available", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AUDIT_LOG_UserId",
                table: "AUDIT_LOG",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_SeatId",
                table: "RESERVATION",
                column: "SeatId");

            migrationBuilder.CreateIndex(
                name: "IX_RESERVATION_UserId",
                table: "RESERVATION",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SEAT_SectorId",
                table: "SEAT",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_SECTOR_EventId",
                table: "SECTOR",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AUDIT_LOG");

            migrationBuilder.DropTable(
                name: "RESERVATION");

            migrationBuilder.DropTable(
                name: "SEAT");

            migrationBuilder.DropTable(
                name: "USER");

            migrationBuilder.DropTable(
                name: "SECTOR");

            migrationBuilder.DropTable(
                name: "EVENT");
        }
    }
}
