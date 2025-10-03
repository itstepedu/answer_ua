using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnswerUA.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangePaymentMethodModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "MethodType",
                table: "PaymentMethods");

            migrationBuilder.RenameColumn(
                name: "WalletEmail",
                table: "PaymentMethods",
                newName: "Last4");

            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "PaymentMethods",
                newName: "Brand");

            migrationBuilder.AddColumn<int>(
                name: "ExpMonth",
                table: "PaymentMethods",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpYear",
                table: "PaymentMethods",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpMonth",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "ExpYear",
                table: "PaymentMethods");

            migrationBuilder.RenameColumn(
                name: "Last4",
                table: "PaymentMethods",
                newName: "WalletEmail");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "PaymentMethods",
                newName: "ExpirationDate");

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "PaymentMethods",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MethodType",
                table: "PaymentMethods",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
