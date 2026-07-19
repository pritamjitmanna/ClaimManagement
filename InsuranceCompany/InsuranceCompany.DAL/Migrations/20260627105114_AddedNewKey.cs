using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InsuranceCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Surveyors",
                keyColumn: "SurveyorId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Surveyors",
                keyColumn: "SurveyorId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Surveyors",
                keyColumn: "SurveyorId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Surveyors",
                keyColumn: "SurveyorId",
                keyValue: 4);

            migrationBuilder.AddColumn<string>(
                name: "SurveyorUserId",
                table: "Surveyors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SurveyorUserId",
                table: "Surveyors");

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
        }
    }
}
