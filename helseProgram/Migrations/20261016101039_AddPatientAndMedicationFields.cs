using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helseProgram.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientAndMedicationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Journal",
                table: "PasientInfo",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PasientHoyde",
                table: "PasientInfo",
                type: "TEXT",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Journal",
                table: "PasientInfo");

            migrationBuilder.DropColumn(
                name: "PasientHoyde",
                table: "PasientInfo");

            migrationBuilder.DropColumn(
                name: "Konsentrasjon",
                table: "Medikamenter");

            migrationBuilder.DropColumn(
                name: "KonsentrasjonEnhet",
                table: "Medikamenter");
        }
    }
}
