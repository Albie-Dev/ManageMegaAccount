using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.Service.Migrations
{
    /// <inheritdoc />
    public partial class ThayDoiCapNhatBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CupSize",
                table: "Actors",
                newName: "Status");

            migrationBuilder.AddColumn<Guid>(
                name: "ExactlyActorId",
                table: "Movies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Links",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Cloud_MegaAccount",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CET_VaiTroNguoiDung",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CET_VaiTro",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CET_TokenNguoiDung",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CET_NguoiDung",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CupSizeType",
                table: "Actors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExactlyActorId",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Links");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cloud_MegaAccount");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CET_VaiTroNguoiDung");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CET_VaiTro");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CET_TokenNguoiDung");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CET_NguoiDung");

            migrationBuilder.DropColumn(
                name: "CupSizeType",
                table: "Actors");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Actors",
                newName: "CupSize");
        }
    }
}
