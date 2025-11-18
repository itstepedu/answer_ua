using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnswerUA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionToAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Addresses",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Region",
                table: "Addresses");
        }
    }
}
