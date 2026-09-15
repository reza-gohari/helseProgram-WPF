using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helseProgram.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedikamentAdministrering",
                columns: table => new
                {
                    MedikamentAdministreringId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MedikamentId = table.Column<int>(type: "INTEGER", nullable: false),
                    PasientId = table.Column<int>(type: "INTEGER", nullable: false),
                    SimSesjonsId = table.Column<int>(type: "INTEGER", nullable: false),
                    AdministreringsType = table.Column<string>(type: "TEXT", nullable: false),
                    AdministreringsMetode = table.Column<string>(type: "TEXT", nullable: false),
                    Dose = table.Column<decimal>(type: "TEXT", nullable: false),
                    Enhet = table.Column<string>(type: "TEXT", nullable: false),
                    StartTid = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndeTid = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Kommentar = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedikamentAdministrering", x => x.MedikamentAdministreringId);
                });

            migrationBuilder.CreateTable(
                name: "Medikamenter",
                columns: table => new
                {
                    MedikamentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MedikamentNavn = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medikamenter", x => x.MedikamentId);
                });

            migrationBuilder.CreateTable(
                name: "PasientInfo",
                columns: table => new
                {
                    PasientId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Fornavn = table.Column<string>(type: "TEXT", nullable: false),
                    Etternavn = table.Column<string>(type: "TEXT", nullable: false),
                    Fodselsdag = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PasientVekt = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasientInfo", x => x.PasientId);
                });

            migrationBuilder.CreateTable(
                name: "SimSesjon",
                columns: table => new
                {
                    SimSesjonsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SesjonsNavn = table.Column<string>(type: "TEXT", nullable: false),
                    StartTid = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndingsTid = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimSesjon", x => x.SimSesjonsId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedikamentAdministrering");

            migrationBuilder.DropTable(
                name: "Medikamenter");

            migrationBuilder.DropTable(
                name: "PasientInfo");

            migrationBuilder.DropTable(
                name: "SimSesjon");
        }
    }
}
