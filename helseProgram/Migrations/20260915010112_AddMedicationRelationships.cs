using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helseProgram.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicationRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MedikamentAdministrering_MedikamentId",
                table: "MedikamentAdministrering",
                column: "MedikamentId");

            migrationBuilder.CreateIndex(
                name: "IX_MedikamentAdministrering_PasientId",
                table: "MedikamentAdministrering",
                column: "PasientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedikamentAdministrering_SimSesjonsId",
                table: "MedikamentAdministrering",
                column: "SimSesjonsId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedikamentAdministrering_Medikamenter_MedikamentId",
                table: "MedikamentAdministrering",
                column: "MedikamentId",
                principalTable: "Medikamenter",
                principalColumn: "MedikamentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedikamentAdministrering_PasientInfo_PasientId",
                table: "MedikamentAdministrering",
                column: "PasientId",
                principalTable: "PasientInfo",
                principalColumn: "PasientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedikamentAdministrering_SimSesjon_SimSesjonsId",
                table: "MedikamentAdministrering",
                column: "SimSesjonsId",
                principalTable: "SimSesjon",
                principalColumn: "SimSesjonsId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedikamentAdministrering_Medikamenter_MedikamentId",
                table: "MedikamentAdministrering");

            migrationBuilder.DropForeignKey(
                name: "FK_MedikamentAdministrering_PasientInfo_PasientId",
                table: "MedikamentAdministrering");

            migrationBuilder.DropForeignKey(
                name: "FK_MedikamentAdministrering_SimSesjon_SimSesjonsId",
                table: "MedikamentAdministrering");

            migrationBuilder.DropIndex(
                name: "IX_MedikamentAdministrering_MedikamentId",
                table: "MedikamentAdministrering");

            migrationBuilder.DropIndex(
                name: "IX_MedikamentAdministrering_PasientId",
                table: "MedikamentAdministrering");

            migrationBuilder.DropIndex(
                name: "IX_MedikamentAdministrering_SimSesjonsId",
                table: "MedikamentAdministrering");
        }
    }
}
