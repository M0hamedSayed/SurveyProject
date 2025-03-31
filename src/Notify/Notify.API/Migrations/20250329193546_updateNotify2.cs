using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notify.API.Migrations
{
    /// <inheritdoc />
    public partial class updateNotify2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_UserID",
                schema: "Notification",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "UserID",
                schema: "Notification",
                table: "notifications");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                schema: "Notification",
                table: "notifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserEmail",
                schema: "Notification",
                table: "notifications",
                column: "UserEmail");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_notifications_UserEmail",
                schema: "Notification",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                schema: "Notification",
                table: "notifications");

            migrationBuilder.AddColumn<Guid>(
                name: "UserID",
                schema: "Notification",
                table: "notifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_notifications_UserID",
                schema: "Notification",
                table: "notifications",
                column: "UserID");
        }
    }
}
