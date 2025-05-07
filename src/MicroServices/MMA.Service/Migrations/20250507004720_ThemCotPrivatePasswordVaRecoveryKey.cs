using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.Service.Migrations
{
    /// <inheritdoc />
    public partial class ThemCotPrivatePasswordVaRecoveryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrivatePassowrd",
                table: "Cloud_MegaAccount",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrivateRecoveryKey",
                table: "Cloud_MegaAccount",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PrivatePassowrd",
                table: "Cloud_MegaAccount");

            migrationBuilder.DropColumn(
                name: "PrivateRecoveryKey",
                table: "Cloud_MegaAccount");
        }
    }
}
