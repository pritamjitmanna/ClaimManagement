using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gateway.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDBsetname : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationModel",
                table: "NotificationModel");

            migrationBuilder.RenameTable(
                name: "NotificationModel",
                newName: "NotificationModels");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationModels",
                table: "NotificationModels",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_NotificationModels",
                table: "NotificationModels");

            migrationBuilder.RenameTable(
                name: "NotificationModels",
                newName: "NotificationModel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_NotificationModel",
                table: "NotificationModel",
                column: "Id");
        }
    }
}
