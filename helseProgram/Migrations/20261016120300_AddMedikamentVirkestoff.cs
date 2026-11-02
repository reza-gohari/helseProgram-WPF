using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helseProgram.Migrations
{
    /// <inheritdoc />
    public partial class AddMedikamentVirkestoff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Konsentrasjon",
                table: "MedikamentType");

            migrationBuilder.DropColumn(
                name: "KonsentrasjonEnhet",
                table: "MedikamentType");

            migrationBuilder.CreateTable(
                name: "MedikamentVirkestoff",
                columns: table => new
                {
                    MedikamentVirkestoffId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MedikamentTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    VirkestoffNavn = table.Column<string>(type: "TEXT", nullable: false),
                    Konsentrasjon = table.Column<decimal>(type: "TEXT", nullable: true),
                    KonsentrasjonEnhet = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedikamentVirkestoff", x => x.MedikamentVirkestoffId);
                    table.ForeignKey(
                        name: "FK_MedikamentVirkestoff_MedikamentType_MedikamentTypeId",
                        column: x => x.MedikamentTypeId,
                        principalTable: "MedikamentType",
                        principalColumn: "MedikamentTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedikamentVirkestoff_MedikamentTypeId",
                table: "MedikamentVirkestoff",
                column: "MedikamentTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedikamentVirkestoff");

            migrationBuilder.AddColumn<decimal>(
                name: "Konsentrasjon",
                table: "MedikamentType",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KonsentrasjonEnhet",
                table: "MedikamentType",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
