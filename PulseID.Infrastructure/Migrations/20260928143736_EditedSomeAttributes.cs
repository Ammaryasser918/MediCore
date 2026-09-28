using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseID.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditedSomeAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labs_AspNetUsers_ApprovedByHospitalAdminId",
                table: "Labs");

            migrationBuilder.DropIndex(
                name: "IX_Labs_ApprovedByHospitalAdminId",
                table: "Labs");

            migrationBuilder.RenameColumn(
                name: "ApprovedByHospitalAdminId",
                table: "Labs",
                newName: "RejectedByLabAdminId");

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedByLabAdminId",
                table: "Labs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "Labs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Labs_ApprovedByLabAdminId",
                table: "Labs",
                column: "ApprovedByLabAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Labs_AspNetUsers_ApprovedByLabAdminId",
                table: "Labs",
                column: "ApprovedByLabAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Labs_AspNetUsers_ApprovedByLabAdminId",
                table: "Labs");

            migrationBuilder.DropIndex(
                name: "IX_Labs_ApprovedByLabAdminId",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "ApprovedByLabAdminId",
                table: "Labs");

            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "Labs");

            migrationBuilder.RenameColumn(
                name: "RejectedByLabAdminId",
                table: "Labs",
                newName: "ApprovedByHospitalAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Labs_ApprovedByHospitalAdminId",
                table: "Labs",
                column: "ApprovedByHospitalAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_Labs_AspNetUsers_ApprovedByHospitalAdminId",
                table: "Labs",
                column: "ApprovedByHospitalAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
