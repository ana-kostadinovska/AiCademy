using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiCademy.Repository.Migrations
{
    /// <inheritdoc />
    public partial class CourseImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Course");
        }
    }
}
