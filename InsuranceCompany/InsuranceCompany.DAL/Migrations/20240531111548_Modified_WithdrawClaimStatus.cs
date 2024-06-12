using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Modified_WithdrawClaimStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "WithdrawClaim",
                table: "ClaimDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "WithdrawClaim",
                table: "ClaimDetails",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);
        }
    }
}
