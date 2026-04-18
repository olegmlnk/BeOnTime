using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BeOnTime.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoadmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Roadmaps",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Roadmaps",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Roadmaps",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TargetDate",
                table: "Roadmaps",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Roadmaps",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_RoadmapId",
                table: "Tasks",
                column: "RoadmapId");

            migrationBuilder.CreateIndex(
                name: "IX_Roadmaps_UserId",
                table: "Roadmaps",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Roadmaps_Users_UserId",
                table: "Roadmaps",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Roadmaps_RoadmapId",
                table: "Tasks",
                column: "RoadmapId",
                principalTable: "Roadmaps",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roadmaps_Users_UserId",
                table: "Roadmaps");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Roadmaps_RoadmapId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_RoadmapId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Roadmaps_UserId",
                table: "Roadmaps");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Roadmaps");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Roadmaps");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Roadmaps");

            migrationBuilder.DropColumn(
                name: "TargetDate",
                table: "Roadmaps");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Roadmaps");
        }
    }
}
