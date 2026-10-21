using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDispensedByForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_Pharmacists_DispensedByPharmacistId",
                table: "PrescriptionItems");

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "PrescriptionItems",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DispensedByUserId",
                table: "PrescriptionItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_DispensedByUserId",
                table: "PrescriptionItems",
                column: "DispensedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_AspNetUsers_DispensedByUserId",
                table: "PrescriptionItems",
                column: "DispensedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_Pharmacists_DispensedByPharmacistId",
                table: "PrescriptionItems",
                column: "DispensedByPharmacistId",
                principalTable: "Pharmacists",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_AspNetUsers_DispensedByUserId",
                table: "PrescriptionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_Pharmacists_DispensedByPharmacistId",
                table: "PrescriptionItems");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionItems_DispensedByUserId",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "DispensedByUserId",
                table: "PrescriptionItems");

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "PrescriptionItems",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_Pharmacists_DispensedByPharmacistId",
                table: "PrescriptionItems",
                column: "DispensedByPharmacistId",
                principalTable: "Pharmacists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
