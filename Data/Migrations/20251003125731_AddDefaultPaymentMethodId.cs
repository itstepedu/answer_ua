using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnswerUA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultPaymentMethodId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultPaymentMethodId",
                table: "AspNetUsers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultPaymentMethodId",
                table: "AspNetUsers");
        }
    }
}
