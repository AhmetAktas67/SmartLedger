using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLedger.Migrations
{
    /// <inheritdoc />
    public partial class AddBeitragszahlung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Beitragszahlungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MitgliedId = table.Column<int>(type: "INTEGER", nullable: false),
                    Jahr = table.Column<int>(type: "INTEGER", nullable: false),
                    Monat = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Kommentar = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Beitragszahlungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Beitragszahlungen_Mitglieder_MitgliedId",
                        column: x => x.MitgliedId,
                        principalTable: "Mitglieder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Beitragszahlungen_MitgliedId",
                table: "Beitragszahlungen",
                column: "MitgliedId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Beitragszahlungen");
        }
    }
}
