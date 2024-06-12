using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Surveyor.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Added_Report_Estimated_loss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstimatedLoss",
                table: "Reports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstimatedLoss",
                table: "Reports");
        }
    }
}
