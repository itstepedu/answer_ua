using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnswerUA.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethodId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PaymentMethods",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethodId",
                table: "PaymentMethods",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "PaymentMethodId",
                table: "PaymentMethods");
        }
    }
}
