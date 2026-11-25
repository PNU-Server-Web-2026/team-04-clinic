using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ClinicAppointments.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicalService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DurationMinutes = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalServices", x => x.Id);
                    table.CheckConstraint("CK_MedicalServices_DurationMinutes", "\"DurationMinutes\" >= 10 AND \"DurationMinutes\" <= 240 AND \"DurationMinutes\" % 5 = 0");
                    table.CheckConstraint("CK_MedicalServices_Price", "\"Price\" >= 0");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalServices_Name",
                table: "MedicalServices",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalServices");
        }
    }
}
