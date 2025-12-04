using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SOSYAL_KUTUPHANE_PLATFORMU.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailedContentFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Authors",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cast",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Genres",
                table: "Contents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageCount",
                table: "Contents",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Authors",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Cast",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Director",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "Genres",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "PageCount",
                table: "Contents");
        }
    }
}
