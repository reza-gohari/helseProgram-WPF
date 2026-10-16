using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helseProgram.Migrations
{
    /// <inheritdoc />
    public partial class AddMedikamentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Konsentrasjon",
                table: "Medikamenter");

            migrationBuilder.DropColumn(
                name: "KonsentrasjonEnhet",
                table: "Medikamenter");

            migrationBuilder.CreateTable(
                name: "MedikamentType",
                columns: table => new
                {
                    MedikamentTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MedikamentId = table.Column<int>(type: "INTEGER", nullable: false),
                    Konsentrasjon = table.Column<decimal>(type: "TEXT", nullable: true),
                    KonsentrasjonEnhet = table.Column<string>(type: "TEXT", nullable: false),
                    Beskrivelse = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedikamentType", x => x.MedikamentTypeId);
                    table.ForeignKey(
                        name: "FK_MedikamentType_Medikamenter_MedikamentId",
                        column: x => x.MedikamentId,
                        principalTable: "Medikamenter",
                        principalColumn: "MedikamentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedikamentType_MedikamentId",
                table: "MedikamentType",
                column: "MedikamentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedikamentType");

            migrationBuilder.AddColumn<decimal>(
                name: "Konsentrasjon",
                table: "Medikamenter",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KonsentrasjonEnhet",
                table: "Medikamenter",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
