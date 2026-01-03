using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gateway.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class IdentityFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "profileId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "profileId",
                table: "AspNetUsers");
        }
    }
}
