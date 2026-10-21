using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPersonalImageForPatient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PersonalImagePath",
                table: "Patients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonalImagePath",
                table: "Patients");
        }
    }
}
