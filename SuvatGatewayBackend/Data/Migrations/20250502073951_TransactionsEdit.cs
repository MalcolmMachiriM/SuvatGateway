using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.DAta.Migrations
{
    /// <inheritdoc />
    public partial class TransactionsEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Success",
                table: "PaymentTransactions");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "PaymentTransactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "PaymentTransactions");

            migrationBuilder.AddColumn<bool>(
                name: "Success",
                table: "PaymentTransactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
