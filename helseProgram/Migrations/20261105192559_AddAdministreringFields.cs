using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace helseProgram.Migrations
{
    /// <inheritdoc />
    public partial class AddAdministreringFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ErKontinuerlig",
                table: "MedikamentAdministrering",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Lokalisasjon",
                table: "MedikamentAdministrering",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "SimulasjonstidTicks",
                table: "MedikamentAdministrering",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErKontinuerlig",
                table: "MedikamentAdministrering");

            migrationBuilder.DropColumn(
                name: "Lokalisasjon",
                table: "MedikamentAdministrering");

            migrationBuilder.DropColumn(
                name: "SimulasjonstidTicks",
                table: "MedikamentAdministrering");
        }
    }
}
