using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiCademy.Repository.Migrations
{
    /// <inheritdoc />
    public partial class favouritesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AspNetUsers_ApplicationUserId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_ApplicationUserId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Lessons");

            migrationBuilder.CreateTable(
                name: "ApplicationUserLesson",
                columns: table => new
                {
                    FavouriteLessonsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserLesson", x => new { x.FavouriteLessonsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserLesson_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserLesson_Lessons_FavouriteLessonsId",
                        column: x => x.FavouriteLessonsId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserLesson_UsersId",
                table: "ApplicationUserLesson",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserLesson");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Lessons",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_ApplicationUserId",
                table: "Lessons",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AspNetUsers_ApplicationUserId",
                table: "Lessons",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
