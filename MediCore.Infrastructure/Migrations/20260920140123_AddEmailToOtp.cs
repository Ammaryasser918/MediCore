using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OTPs_UserId_Purpose_IsUsed",
                table: "OTPs");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "OTPs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OTPs",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_OTPs_Email_Purpose_IsUsed",
                table: "OTPs",
                columns: new[] { "Email", "Purpose", "IsUsed" });

            migrationBuilder.CreateIndex(
                name: "IX_OTPs_UserId",
                table: "OTPs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OTPs_Email_Purpose_IsUsed",
                table: "OTPs");

            migrationBuilder.DropIndex(
                name: "IX_OTPs_UserId",
                table: "OTPs");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "OTPs");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "OTPs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OTPs_UserId_Purpose_IsUsed",
                table: "OTPs",
                columns: new[] { "UserId", "Purpose", "IsUsed" });
        }
    }
}
