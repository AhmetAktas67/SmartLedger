using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLedger.Migrations
{
    /// <inheritdoc />
    public partial class FixBuchungProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mitglieder_Buchungen_BuchungId",
                table: "Mitglieder");

            migrationBuilder.DropIndex(
                name: "IX_Mitglieder_BuchungId",
                table: "Mitglieder");

            migrationBuilder.DropColumn(
                name: "BuchungId",
                table: "Mitglieder");

            migrationBuilder.AddColumn<string>(
                name: "ZugeordneteMitgliederNamen",
                table: "Buchungen",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ZugeordneteMitgliederNamen",
                table: "Buchungen");

            migrationBuilder.AddColumn<int>(
                name: "BuchungId",
                table: "Mitglieder",
                type: "INTEGER",
                nullable: true);

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
    }
}
