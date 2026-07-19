using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Remove_SurveyorId_Field_Fully : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SurveyorID",
                table: "ClaimDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SurveyorID",
                table: "ClaimDetails",
                type: "int",
                nullable: true);
        }
    }
}
