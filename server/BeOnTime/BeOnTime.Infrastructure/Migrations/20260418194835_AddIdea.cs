using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeOnTime.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Ideas",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsConvertedToTask",
                table: "Ideas",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Ideas",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Ideas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Ideas_UserId",
                table: "Ideas",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ideas_Users_UserId",
                table: "Ideas",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ideas_Users_UserId",
                table: "Ideas");

            migrationBuilder.DropIndex(
                name: "IX_Ideas_UserId",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "IsConvertedToTask",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Ideas");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Ideas");
        }
    }
}
