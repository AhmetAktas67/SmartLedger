using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLedger.Migrations
{
    /// <inheritdoc />
    public partial class AddBuchungenTabelle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BuchungId",
                table: "Mitglieder",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Buchungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BuchungsDatum = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Verwendungszweck = table.Column<string>(type: "TEXT", nullable: false),
                    Betrag = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buchungen", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Mitglieder_BuchungId",
                table: "Mitglieder",
                column: "BuchungId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mitglieder_Buchungen_BuchungId",
                table: "Mitglieder",
                column: "BuchungId",
                principalTable: "Buchungen",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mitglieder_Buchungen_BuchungId",
                table: "Mitglieder");

            migrationBuilder.DropTable(
                name: "Buchungen");

            migrationBuilder.DropIndex(
                name: "IX_Mitglieder_BuchungId",
                table: "Mitglieder");

            migrationBuilder.DropColumn(
                name: "BuchungId",
                table: "Mitglieder");
        }
    }
}
