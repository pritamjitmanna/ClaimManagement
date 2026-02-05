using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace InsuranceCompany.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Added_Policy_UserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Policies",
                keyColumn: "PolicyNo",
                keyValue: "MA33724");

            migrationBuilder.DeleteData(
                table: "Policies",
                keyColumn: "PolicyNo",
                keyValue: "PR88624");

            migrationBuilder.DeleteData(
                table: "Policies",
                keyColumn: "PolicyNo",
                keyValue: "SO18824");

            migrationBuilder.AddColumn<string>(
                name: "PolicyUserId",
                table: "Policies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PolicyUserId",
                table: "Policies");

            migrationBuilder.InsertData(
                table: "Policies",
                columns: new[] { "PolicyNo", "DateOfInsurance", "EmailId", "InsuredFirstName", "InsuredLastName", "VehicleNo", "status" },
                values: new object[,]
                {
                    { "MA33724", new DateOnly(2024, 3, 14), "pm@cognizant.com", "Pritamjit", "Manna", "HR26DK8337", true },
                    { "PR88624", new DateOnly(2024, 3, 13), "mp@cognizant.com", "Manna", "Pritamjit", "WB31W6886", true },
                    { "SO18824", new DateOnly(2024, 3, 15), "ms@cognizant.com", "Souvik", "Maity", "WB32U3188", false }
                });
        }
    }
}
