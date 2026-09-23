using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseID.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicationsAndPrescriptionUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MedicationCode",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "MedicationName",
                table: "PrescriptionItems");

            migrationBuilder.AddColumn<Guid>(
                name: "MedicationId",
                table: "PrescriptionItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Medications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommercialNameEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CommercialNameAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ScientificName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DrugClass = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Route = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PriceEgp = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_MedicationId",
                table: "PrescriptionItems",
                column: "MedicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Medications_CommercialNameEn",
                table: "Medications",
                column: "CommercialNameEn");

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_Medications_MedicationId",
                table: "PrescriptionItems",
                column: "MedicationId",
                principalTable: "Medications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_Medications_MedicationId",
                table: "PrescriptionItems");

            migrationBuilder.DropTable(
                name: "Medications");

            migrationBuilder.DropIndex(
                name: "IX_PrescriptionItems_MedicationId",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "MedicationId",
                table: "PrescriptionItems");

            migrationBuilder.AddColumn<string>(
                name: "MedicationCode",
                table: "PrescriptionItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MedicationName",
                table: "PrescriptionItems",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }
    }
}
