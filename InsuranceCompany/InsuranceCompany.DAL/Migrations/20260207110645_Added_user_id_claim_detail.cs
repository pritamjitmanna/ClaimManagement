using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Added_user_id_claim_detail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClaimUserId",
                table: "ClaimDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimUserId",
                table: "ClaimDetails");
        }
    }
}
