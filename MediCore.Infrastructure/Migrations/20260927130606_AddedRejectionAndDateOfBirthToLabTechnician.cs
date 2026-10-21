using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRejectionAndDateOfBirthToLabTechnician : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTechnicians_AspNetUsers_ApprovedByHospitalAdminId",
                table: "LabTechnicians");

            migrationBuilder.DropIndex(
                name: "IX_LabTechnicians_ApprovedByHospitalAdminId",
                table: "LabTechnicians");

            migrationBuilder.RenameColumn(
                name: "ApprovedByHospitalAdminId",
                table: "LabTechnicians",
                newName: "RejectedByLabAdminId");

            migrationBuilder.AlterColumn<Guid>(
                name: "LabId",
                table: "LabTechnicians",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedByLabAdminId",
                table: "LabTechnicians",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                table: "LabTechnicians",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "LabTechnicians",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTechnicians_ApprovedByLabAdminId",
                table: "LabTechnicians",
                column: "ApprovedByLabAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTechnicians_AspNetUsers_ApprovedByLabAdminId",
                table: "LabTechnicians",
                column: "ApprovedByLabAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LabTechnicians_AspNetUsers_ApprovedByLabAdminId",
                table: "LabTechnicians");

            migrationBuilder.DropIndex(
                name: "IX_LabTechnicians_ApprovedByLabAdminId",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "ApprovedByLabAdminId",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "LabTechnicians");

            migrationBuilder.RenameColumn(
                name: "RejectedByLabAdminId",
                table: "LabTechnicians",
                newName: "ApprovedByHospitalAdminId");

            migrationBuilder.AlterColumn<Guid>(
                name: "LabId",
                table: "LabTechnicians",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTechnicians_ApprovedByHospitalAdminId",
                table: "LabTechnicians",
                column: "ApprovedByHospitalAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_LabTechnicians_AspNetUsers_ApprovedByHospitalAdminId",
                table: "LabTechnicians",
                column: "ApprovedByHospitalAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
