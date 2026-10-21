using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedRejectedAtAndBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Pharmacists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RejectedByAdminId",
                table: "Pharmacists",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Pharmacies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RejectedByAdminId",
                table: "Pharmacies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Doctors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RejectedByAdminId",
                table: "Doctors",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Pharmacists");

            migrationBuilder.DropColumn(
                name: "RejectedByAdminId",
                table: "Pharmacists");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "RejectedByAdminId",
                table: "Pharmacies");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "RejectedByAdminId",
                table: "Doctors");
        }
    }
}
