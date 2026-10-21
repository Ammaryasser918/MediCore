using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDateOfBirthToPharmacistAndDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Pharmacists",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Pharmacists",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "Doctors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Pharmacists");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Pharmacists");

            migrationBuilder.DropColumn(
                name: "Age",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "Doctors");
        }
    }
}
