using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gateway.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Fully_Remove_profileId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profileId",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "profileId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }
    }
}
