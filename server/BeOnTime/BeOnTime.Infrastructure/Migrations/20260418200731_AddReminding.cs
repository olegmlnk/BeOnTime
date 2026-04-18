using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeOnTime.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddReminding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "Remindings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemindAt",
                table: "Remindings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Remindings",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                table: "Remindings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Remindings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Remindings_Status_RemindAt",
                table: "Remindings",
                columns: new[] { "Status", "RemindAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Remindings_TaskId",
                table: "Remindings",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_Remindings_UserId",
                table: "Remindings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Remindings_Tasks_TaskId",
                table: "Remindings",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Remindings_Users_UserId",
                table: "Remindings",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Remindings_Tasks_TaskId",
                table: "Remindings");

            migrationBuilder.DropForeignKey(
                name: "FK_Remindings_Users_UserId",
                table: "Remindings");

            migrationBuilder.DropIndex(
                name: "IX_Remindings_Status_RemindAt",
                table: "Remindings");

            migrationBuilder.DropIndex(
                name: "IX_Remindings_TaskId",
                table: "Remindings");

            migrationBuilder.DropIndex(
                name: "IX_Remindings_UserId",
                table: "Remindings");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "Remindings");

            migrationBuilder.DropColumn(
                name: "RemindAt",
                table: "Remindings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Remindings");

            migrationBuilder.DropColumn(
                name: "TaskId",
                table: "Remindings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Remindings");
        }
    }
}
