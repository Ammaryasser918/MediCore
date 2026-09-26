using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseID.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedShiftStartEndtoPharmacist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ShiftEnded",
                table: "Pharmacists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ShiftStarted",
                table: "Pharmacists",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShiftEnded",
                table: "Pharmacists");

            migrationBuilder.DropColumn(
                name: "ShiftStarted",
                table: "Pharmacists");
        }
    }
}
