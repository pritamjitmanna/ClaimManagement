using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Assign_SurveyorUserId_NewPrimayKey_ForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Fk_Surveyor_ClaimDetail",
                table: "ClaimDetails");

            migrationBuilder.DropPrimaryKey(
                name: "Pk_Surveyor",
                table: "Surveyors");

            migrationBuilder.DropIndex(
                name: "IX_ClaimDetails_SurveyorID",
                table: "ClaimDetails");

            migrationBuilder.AlterColumn<string>(
                name: "SurveyorUserId",
                table: "Surveyors",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "SurveyorUserId",
                table: "ClaimDetails",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "Pk_Surveyor",
                table: "Surveyors",
                column: "SurveyorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDetails_SurveyorUserId",
                table: "ClaimDetails",
                column: "SurveyorUserId");

            migrationBuilder.AddForeignKey(
                name: "Fk_Surveyor_ClaimDetail",
                table: "ClaimDetails",
                column: "SurveyorUserId",
                principalTable: "Surveyors",
                principalColumn: "SurveyorUserId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "Fk_Surveyor_ClaimDetail",
                table: "ClaimDetails");

            migrationBuilder.DropPrimaryKey(
                name: "Pk_Surveyor",
                table: "Surveyors");

            migrationBuilder.DropIndex(
                name: "IX_ClaimDetails_SurveyorUserId",
                table: "ClaimDetails");

            migrationBuilder.DropColumn(
                name: "SurveyorUserId",
                table: "ClaimDetails");

            migrationBuilder.AlterColumn<string>(
                name: "SurveyorUserId",
                table: "Surveyors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "Pk_Surveyor",
                table: "Surveyors",
                column: "SurveyorId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDetails_SurveyorID",
                table: "ClaimDetails",
                column: "SurveyorID");

            migrationBuilder.AddForeignKey(
                name: "Fk_Surveyor_ClaimDetail",
                table: "ClaimDetails",
                column: "SurveyorID",
                principalTable: "Surveyors",
                principalColumn: "SurveyorId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
