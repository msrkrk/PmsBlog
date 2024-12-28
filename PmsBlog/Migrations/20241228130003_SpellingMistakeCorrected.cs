using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PmsBlog.Migrations
{
    /// <inheritdoc />
    public partial class SpellingMistakeCorrected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "readingCount",
                table: "Articles",
                newName: "ReadingCount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReadingCount",
                table: "Articles",
                newName: "readingCount");
        }
    }
}
