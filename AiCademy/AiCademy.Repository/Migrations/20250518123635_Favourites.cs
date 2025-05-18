using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiCademy.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Favourites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
