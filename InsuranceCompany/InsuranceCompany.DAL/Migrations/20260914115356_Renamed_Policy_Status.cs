using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Renamed_Policy_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "Policies",
                newName: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Policies",
                newName: "status");
        }
    }
}
