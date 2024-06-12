using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InsuranceCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Added_All_Tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fees",
                columns: table => new
                {
                    FeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstimateStartLimit = table.Column<int>(type: "int", nullable: false),
                    EstimateEndLimit = table.Column<int>(type: "int", nullable: false),
                    fees = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Pk_Fee", x => x.FeeId);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    PolicyNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InsuredFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsuredLastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfInsurance = table.Column<DateOnly>(type: "date", nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Pk_Policy", x => x.PolicyNo);
                });

            migrationBuilder.CreateTable(
                name: "Surveyors",
                columns: table => new
                {
                    SurveyorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimateLimit = table.Column<int>(type: "int", nullable: false),
                    TimesAllocated = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Pk_Surveyor", x => x.SurveyorId);
                });

            migrationBuilder.CreateTable(
                name: "ClaimDetails",
                columns: table => new
                {
                    ClaimId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PolicyNo = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EstimatedLoss = table.Column<int>(type: "int", nullable: false),
                    DateOfAccident = table.Column<DateOnly>(type: "date", nullable: false),
                    ClaimStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    SurveyorID = table.Column<int>(type: "int", nullable: true),
                    AmtApprovedBySurveyor = table.Column<int>(type: "int", nullable: true),
                    InsuranceCompanyApproval = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    WithdrawClaim = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SurveyorFees = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Pk_ClaimDetail", x => x.ClaimId);
                    table.ForeignKey(
                        name: "Fk_Policy_ClaimDetail",
                        column: x => x.PolicyNo,
                        principalTable: "Policies",
                        principalColumn: "PolicyNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "Fk_Surveyor_ClaimDetail",
                        column: x => x.SurveyorID,
                        principalTable: "Surveyors",
                        principalColumn: "SurveyorId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Fees",
                columns: new[] { "FeeId", "EstimateEndLimit", "EstimateStartLimit", "fees" },
                values: new object[,]
                {
                    { 1, 10000, 5000, 1000 },
                    { 2, 20000, 10000, 2000 },
                    { 3, 70000, 20000, 7000 }
                });

            migrationBuilder.InsertData(
                table: "Policies",
                columns: new[] { "PolicyNo", "DateOfInsurance", "EmailId", "InsuredFirstName", "InsuredLastName", "VehicleNo", "status" },
                values: new object[,]
                {
                    { "MA33724", new DateOnly(2024, 3, 14), "pm@cognizant.com", "Pritamjit", "Manna", "HR26DK8337", true },
                    { "PR88624", new DateOnly(2024, 3, 13), "mp@cognizant.com", "Manna", "Pritamjit", "WB31W6886", true },
                    { "SO18824", new DateOnly(2024, 3, 15), "ms@cognizant.com", "Souvik", "Maity", "WB32U3188", false }
                });

            migrationBuilder.InsertData(
                table: "Surveyors",
                columns: new[] { "SurveyorId", "EstimateLimit", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, 6000, "R", "K" },
                    { 2, 15000, "K", "R" },
                    { 3, 50000, "P", "M" },
                    { 4, 15000, "S", "M" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDetails_PolicyNo",
                table: "ClaimDetails",
                column: "PolicyNo");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDetails_SurveyorID",
                table: "ClaimDetails",
                column: "SurveyorID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClaimDetails");

            migrationBuilder.DropTable(
                name: "Fees");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "Surveyors");
        }
    }
}
