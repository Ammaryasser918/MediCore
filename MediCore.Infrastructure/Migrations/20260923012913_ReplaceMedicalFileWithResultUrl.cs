using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceMedicalFileWithResultUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalFiles_LabTestRequests_LabTestRequestId",
                table: "MedicalFiles");

            migrationBuilder.DropIndex(
                name: "IX_MedicalFiles_LabTestRequestId",
                table: "MedicalFiles");

            migrationBuilder.DropColumn(
                name: "LabTestRequestId",
                table: "MedicalFiles");

            migrationBuilder.AddColumn<string>(
                name: "ResultFileUrl",
                table: "LabTestRequests",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResultFileUrl",
                table: "LabTestRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "LabTestRequestId",
                table: "MedicalFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MedicalFiles_LabTestRequestId",
                table: "MedicalFiles",
                column: "LabTestRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalFiles_LabTestRequests_LabTestRequestId",
                table: "MedicalFiles",
                column: "LabTestRequestId",
                principalTable: "LabTestRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
