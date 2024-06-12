using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surveyor.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Added_SurveyReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    ClaimId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PolicyNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LabourCharges = table.Column<int>(type: "int", nullable: false),
                    PartsCost = table.Column<int>(type: "int", nullable: false),
                    PolicyClause = table.Column<int>(type: "int", nullable: false),
                    DepreciationCost = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<int>(type: "int", nullable: false),
                    AccidentDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VehicleAge = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Pk_SurveyReport", x => x.ClaimId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reports");
        }
    }
}
