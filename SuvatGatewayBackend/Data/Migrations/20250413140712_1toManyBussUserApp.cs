using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.DAta.Migrations
{
    /// <inheritdoc />
    public partial class _1toManyBussUserApp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "Users",
                newName: "AppBusinessId");

            migrationBuilder.RenameColumn(
                name: "BusinessId",
                table: "Applications",
                newName: "AppBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AppBusinessId",
                table: "Users",
                column: "AppBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_AppBusinessId",
                table: "Applications",
                column: "AppBusinessId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Businesses_AppBusinessId",
                table: "Applications",
                column: "AppBusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Businesses_AppBusinessId",
                table: "Users",
                column: "AppBusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Businesses_AppBusinessId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Businesses_AppBusinessId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AppBusinessId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Applications_AppBusinessId",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "AppBusinessId",
                table: "Users",
                newName: "BusinessId");

            migrationBuilder.RenameColumn(
                name: "AppBusinessId",
                table: "Applications",
                newName: "BusinessId");
        }
    }
}
