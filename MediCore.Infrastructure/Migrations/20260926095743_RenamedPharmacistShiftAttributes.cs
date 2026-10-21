using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedPharmacistShiftAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShiftStarted",
                table: "Pharmacists",
                newName: "ShiftStartedAt");

            migrationBuilder.RenameColumn(
                name: "ShiftEnded",
                table: "Pharmacists",
                newName: "ShiftEndedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShiftStartedAt",
                table: "Pharmacists",
                newName: "ShiftStarted");

            migrationBuilder.RenameColumn(
                name: "ShiftEndedAt",
                table: "Pharmacists",
                newName: "ShiftEnded");
        }
    }
}
