using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLabModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DispensedAt",
                table: "Prescriptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LabTestRequestId",
                table: "MedicalFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Labs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LicenseDocumentPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedByHospitalAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Labs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Labs_AspNetUsers_ApprovedByHospitalAdminId",
                        column: x => x.ApprovedByHospitalAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LabTechnicians",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LicenseDocumentPath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonalImagePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LabId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApprovedByHospitalAdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTechnicians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTechnicians_AspNetUsers_ApprovedByHospitalAdminId",
                        column: x => x.ApprovedByHospitalAdminId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTechnicians_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTechnicians_Labs_LabId",
                        column: x => x.LabId,
                        principalTable: "Labs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabTestRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabTestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SampleCollectedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTestRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabTestRequests_Doctors_DoctorId",
                        column: x => x.DoctorId,
                        principalTable: "Doctors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTestRequests_LabTests_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTestRequests_Labs_LabId",
                        column: x => x.LabId,
                        principalTable: "Labs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabTestRequests_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalFiles_LabTestRequestId",
                table: "MedicalFiles",
                column: "LabTestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Labs_ApprovedByHospitalAdminId",
                table: "Labs",
                column: "ApprovedByHospitalAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTechnicians_ApprovedByHospitalAdminId",
                table: "LabTechnicians",
                column: "ApprovedByHospitalAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTechnicians_LabId",
                table: "LabTechnicians",
                column: "LabId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTechnicians_LicenseNumber",
                table: "LabTechnicians",
                column: "LicenseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTechnicians_NationalId",
                table: "LabTechnicians",
                column: "NationalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTechnicians_UserId",
                table: "LabTechnicians",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabTestRequests_DoctorId",
                table: "LabTestRequests",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestRequests_LabId",
                table: "LabTestRequests",
                column: "LabId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestRequests_LabTestId",
                table: "LabTestRequests",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTestRequests_PatientId",
                table: "LabTestRequests",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_Name",
                table: "LabTests",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalFiles_LabTestRequests_LabTestRequestId",
                table: "MedicalFiles",
                column: "LabTestRequestId",
                principalTable: "LabTestRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalFiles_LabTestRequests_LabTestRequestId",
                table: "MedicalFiles");

            migrationBuilder.DropTable(
                name: "LabTechnicians");

            migrationBuilder.DropTable(
                name: "LabTestRequests");

            migrationBuilder.DropTable(
                name: "LabTests");

            migrationBuilder.DropTable(
                name: "Labs");

            migrationBuilder.DropIndex(
                name: "IX_MedicalFiles_LabTestRequestId",
                table: "MedicalFiles");

            migrationBuilder.DropColumn(
                name: "DispensedAt",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "LabTestRequestId",
                table: "MedicalFiles");
        }
    }
}
