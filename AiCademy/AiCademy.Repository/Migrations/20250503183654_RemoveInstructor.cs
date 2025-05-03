using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiCademy.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveInstructor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_AspNetUsers_InstructorId1",
                table: "Course");

            migrationBuilder.DropIndex(
                name: "IX_Course_InstructorId1",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "InstructorId1",
                table: "Course");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InstructorId",
                table: "Course",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstructorId1",
                table: "Course",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Course_InstructorId1",
                table: "Course",
                column: "InstructorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_AspNetUsers_InstructorId1",
                table: "Course",
                column: "InstructorId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
