using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.DAta.Migrations
{
    /// <inheritdoc />
    public partial class TransactionsEdit1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "PaymentTransactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "PaymentTransactions");
        }
    }
}
