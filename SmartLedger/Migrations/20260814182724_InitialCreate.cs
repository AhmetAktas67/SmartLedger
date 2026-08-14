using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartLedger.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Mitglieder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Vorname = table.Column<string>(type: "TEXT", nullable: false),
                    Nachname = table.Column<string>(type: "TEXT", nullable: false),
                    Monatsbeitrag = table.Column<decimal>(type: "TEXT", nullable: false),
                    Haushaltsgruppe = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mitglieder", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Mitglieder");
        }
    }
}
