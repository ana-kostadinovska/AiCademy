using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AiCademy.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class LessonModels2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PdfContent",
                table: "Lessons",
                newName: "FilePath");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Lessons",
                newName: "PdfContent");
        }
    }
}
