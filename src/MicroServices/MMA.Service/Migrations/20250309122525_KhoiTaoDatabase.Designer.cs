﻿// <auto-generated />
using System;
using MMA.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MMA.Service.Migrations
{
    [DbContext(typeof(MMADbContext))]
    [Migration("20250309122525_KhoiTaoDatabase")]
    partial class KhoiTaoDatabase
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("MMA.Service.LinkHelperEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Client_AppEndpoint")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Client_ConfirmNewPasswordRoute")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiTaoId");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgayTao");

                    b.Property<Guid>("ModifiedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiSuaDoiId");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgaySuaDoi");

                    b.Property<string>("Server_AppEndpoint")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Links");
                });

            modelBuilder.Entity("MMA.Service.MegaAccountEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiTaoId");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgayTao");

                    b.Property<DateTimeOffset>("ExpiredDate")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("Files")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("LastLogin")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("ModifiedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiSuaDoiId");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgaySuaDoi");

                    b.Property<string>("PasswordHashed")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RecoveryKeyHashed")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("TotalFileSize")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Cloud_MegaAccount");
                });

            modelBuilder.Entity("MMA.Service.RoleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiTaoId");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgayTao");

                    b.Property<Guid>("ModifiedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiSuaDoiId");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgaySuaDoi");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("TenVaiTro");

                    b.Property<int>("RoleType")
                        .HasColumnType("int")
                        .HasColumnName("LoaiVaiTro");

                    b.HasKey("Id");

                    b.HasIndex("RoleName")
                        .IsUnique();

                    b.HasIndex("RoleType")
                        .IsUnique();

                    b.ToTable("CET_VaiTro");
                });

            modelBuilder.Entity("MMA.Service.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("AnhDaiDien");

                    b.Property<int>("CountLoginFailed")
                        .HasColumnType("int")
                        .HasColumnName("SoLanDangNhapThatBai");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiTaoId");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgayTao");

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date")
                        .HasColumnName("NgaySinh");

                    b.Property<string>("DefaultAddress")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("DiaChiMacDinh");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("EmailConfirm")
                        .HasColumnType("bit")
                        .HasColumnName("XacThucEmail");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("HoTen");

                    b.Property<bool>("IsAccountLocked")
                        .HasColumnType("bit")
                        .HasColumnName("KhoaTaiKhoan");

                    b.Property<Guid>("ModifiedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiSuaDoiId");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgaySuaDoi");

                    b.Property<string>("OldPasswords")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("MatKhauCu");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasColumnName("MatKhauMaHoa");

                    b.Property<bool>("PhoneConfirm")
                        .HasColumnType("bit")
                        .HasColumnName("XacThucSoDienThoai");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("SoDienThoai");

                    b.Property<bool>("TwoFactorEnable")
                        .HasColumnType("bit")
                        .HasColumnName("XacThucHaiYeuTo");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("TenDangNhap");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("FullName");

                    b.HasIndex("PhoneNumber");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("CET_NguoiDung", (string)null);
                });

            modelBuilder.Entity("MMA.Service.UserRoleEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiDungId");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("VaiTroId");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiTaoId");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgayTao");

                    b.Property<Guid>("ModifiedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiSuaDoiId");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgaySuaDoi");

                    b.Property<string>("RolePermissionProperty")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("ChiTietVaiTro");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("CET_VaiTroNguoiDung", (string)null);
                });

            modelBuilder.Entity("MMA.Service.UserTokenEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CreatedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiTaoId");

                    b.Property<DateTimeOffset>("CreatedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgayTao");

                    b.Property<DateTimeOffset>("ExpiredDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgayHetHan");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("bit")
                        .HasColumnName("DaThuHoi");

                    b.Property<int>("MaxUse")
                        .HasColumnType("int")
                        .HasColumnName("SoLanSuDungToiDa");

                    b.Property<Guid>("ModifiedBy")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiSuaDoiId");

                    b.Property<DateTimeOffset>("ModifiedDate")
                        .HasColumnType("datetimeoffset")
                        .HasColumnName("NgaySuaDoi");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("TokenLamMoi");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)")
                        .HasColumnName("TokenXacThuc");

                    b.Property<int>("TokenType")
                        .HasColumnType("int")
                        .HasColumnName("LoaiToken");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("NguoiDungId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("CET_TokenNguoiDung");
                });

            modelBuilder.Entity("MMA.Service.UserRoleEntity", b =>
                {
                    b.HasOne("MMA.Service.RoleEntity", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("MMA.Service.UserEntity", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("MMA.Service.UserTokenEntity", b =>
                {
                    b.HasOne("MMA.Service.UserEntity", "User")
                        .WithMany("UserTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("MMA.Service.RoleEntity", b =>
                {
                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("MMA.Service.UserEntity", b =>
                {
                    b.Navigation("UserRoles");

                    b.Navigation("UserTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
