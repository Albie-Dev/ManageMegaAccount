using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.Service.Migrations
{
    /// <inheritdoc />
    public partial class KhoiTaoDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CET_NguoiDung",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhauMaHoa = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MatKhauCu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XacThucEmail = table.Column<bool>(type: "bit", nullable: false),
                    XacThucHaiYeuTo = table.Column<bool>(type: "bit", nullable: false),
                    XacThucSoDienThoai = table.Column<bool>(type: "bit", nullable: false),
                    SoLanDangNhapThatBai = table.Column<int>(type: "int", nullable: false),
                    KhoaTaiKhoan = table.Column<bool>(type: "bit", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: false),
                    DiaChiMacDinh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NguoiTaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiSuaDoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgaySuaDoi = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CET_NguoiDung", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CET_VaiTro",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenVaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LoaiVaiTro = table.Column<int>(type: "int", nullable: false),
                    NguoiTaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiSuaDoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgaySuaDoi = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CET_VaiTro", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cloud_MegaAccount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHashed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecoveryKeyHashed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastLogin = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpiredDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TotalFileSize = table.Column<double>(type: "float", nullable: false),
                    Files = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NguoiTaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiSuaDoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgaySuaDoi = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cloud_MegaAccount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Links",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Server_AppEndpoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Client_AppEndpoint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Client_ConfirmNewPasswordRoute = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NguoiTaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiSuaDoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgaySuaDoi = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Links", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CET_TokenNguoiDung",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenXacThuc = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    SoLanSuDungToiDa = table.Column<int>(type: "int", nullable: false),
                    TokenLamMoi = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    LoaiToken = table.Column<int>(type: "int", nullable: false),
                    NgayHetHan = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DaThuHoi = table.Column<bool>(type: "bit", nullable: false),
                    NguoiDungId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiTaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiSuaDoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgaySuaDoi = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CET_TokenNguoiDung", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CET_TokenNguoiDung_CET_NguoiDung_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "CET_NguoiDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CET_VaiTroNguoiDung",
                columns: table => new
                {
                    NguoiDungId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VaiTroId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ChiTietVaiTro = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NguoiTaoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NguoiSuaDoiId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NgayTao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    NgaySuaDoi = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CET_VaiTroNguoiDung", x => new { x.NguoiDungId, x.VaiTroId });
                    table.ForeignKey(
                        name: "FK_CET_VaiTroNguoiDung_CET_NguoiDung_NguoiDungId",
                        column: x => x.NguoiDungId,
                        principalTable: "CET_NguoiDung",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CET_VaiTroNguoiDung_CET_VaiTro_VaiTroId",
                        column: x => x.VaiTroId,
                        principalTable: "CET_VaiTro",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CET_NguoiDung_Email",
                table: "CET_NguoiDung",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CET_NguoiDung_HoTen",
                table: "CET_NguoiDung",
                column: "HoTen");

            migrationBuilder.CreateIndex(
                name: "IX_CET_NguoiDung_SoDienThoai",
                table: "CET_NguoiDung",
                column: "SoDienThoai");

            migrationBuilder.CreateIndex(
                name: "IX_CET_NguoiDung_TenDangNhap",
                table: "CET_NguoiDung",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CET_TokenNguoiDung_NguoiDungId",
                table: "CET_TokenNguoiDung",
                column: "NguoiDungId");

            migrationBuilder.CreateIndex(
                name: "IX_CET_VaiTro_LoaiVaiTro",
                table: "CET_VaiTro",
                column: "LoaiVaiTro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CET_VaiTro_TenVaiTro",
                table: "CET_VaiTro",
                column: "TenVaiTro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CET_VaiTroNguoiDung_VaiTroId",
                table: "CET_VaiTroNguoiDung",
                column: "VaiTroId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CET_TokenNguoiDung");

            migrationBuilder.DropTable(
                name: "CET_VaiTroNguoiDung");

            migrationBuilder.DropTable(
                name: "Cloud_MegaAccount");

            migrationBuilder.DropTable(
                name: "Links");

            migrationBuilder.DropTable(
                name: "CET_NguoiDung");

            migrationBuilder.DropTable(
                name: "CET_VaiTro");
        }
    }
}
