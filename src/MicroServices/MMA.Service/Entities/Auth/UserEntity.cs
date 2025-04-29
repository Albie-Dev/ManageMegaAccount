using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MMA.Domain;

namespace MMA.Service
{
    [Table(name: "CET_NguoiDung")]
    public class UserEntity : BaseEntity
    {
        #region auth info
        [Column(name: "TenDangNhap")]
        [StringLength(maximumLength: 50)]
        public string UserName { get; set; } = string.Empty;
        [StringLength(maximumLength: 50)]
        public string Email { get; set; } = string.Empty;
        [Column(name: "MatKhauMaHoa")]
        [StringLength(maximumLength: 256)]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("MatKhauCu")]
        public string OldPasswords { get; set; } = string.Empty;
        [NotMapped]
        public List<PasswordProperty> PasswordProperties
        {
            get => OldPasswords.FromJson<List<PasswordProperty>>();
            set => OldPasswords = PasswordProperties.ToJson();
        }
        [Column(name: "XacThucEmail")]
        public bool EmailConfirm { get; set; }
        [Column(name: "XacThucHaiYeuTo")]
        public bool TwoFactorEnable { get; set; }
        [Column(name: "XacThucSoDienThoai")]
        public bool PhoneConfirm { get; set; }
        [Column(name: "SoLanDangNhapThatBai")]
        public int CountLoginFailed { get; set; }
        [Column(name: "KhoaTaiKhoan")]
        public bool IsAccountLocked { get; set; }
        #endregion auth info

        #region personal info   
        [Column(name: "HoTen")]
        [StringLength(maximumLength: 100)]
        public string FullName { get; set; } = string.Empty;
        [Column(name: "NgaySinh")]
        public DateOnly DateOfBirth { get; set; }
        [Column(name: "DiaChiMacDinh")]
        [StringLength(maximumLength: 500)]
        public string DefaultAddress { get; set; } = string.Empty;
        [Column(name: "SoDienThoai")]
        [StringLength(maximumLength: 20)]
        public string PhoneNumber { get; set; } = string.Empty;
        [Column(name: "AnhDaiDien")]
        [StringLength(maximumLength: 500)]
        public string Avatar { get; set; } = string.Empty;
        #endregion personal info


        [InverseProperty(nameof(UserRoleEntity.User))]
        public virtual ICollection<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
        [InverseProperty(nameof(UserTokenEntity.User))]
        public virtual ICollection<UserTokenEntity> UserTokens { get; set; } = new List<UserTokenEntity>();
        [InverseProperty(nameof(NotificationEntity.Owner))]
        public virtual ICollection<NotificationEntity> Notifications { get; set; } = new List<NotificationEntity>();
        [InverseProperty(nameof(NotificationEntity.Sender))]
        public virtual ICollection<NotificationEntity> SentNotifications { get; set; } = new List<NotificationEntity>();
    }
}