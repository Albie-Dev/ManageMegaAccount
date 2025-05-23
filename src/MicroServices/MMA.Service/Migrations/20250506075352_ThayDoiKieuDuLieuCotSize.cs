using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.Service.Migrations
{
    /// <inheritdoc />
    public partial class ThayDoiKieuDuLieuCotSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TotalFileSize",
                table: "Cloud_MegaAccount",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TotalFileSize",
                table: "Cloud_MegaAccount",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
