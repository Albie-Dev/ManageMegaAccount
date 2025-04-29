using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MMA.Domain;

namespace MMA.Service
{
    [Table(name: "CET_TokenNguoiDung")]
    public class UserTokenEntity : BaseEntity
    {
        [Column("TokenXacThuc")]
        [StringLength(maximumLength: 5000)]
        public string Token { get; set; } = string.Empty;
        [Column("SoLanSuDungToiDa")]
        public int MaxUse { get; set; }
        [Column("TokenLamMoi")]
        [StringLength(maximumLength: 5000)]
        public string RefreshToken { get; set; } = string.Empty;
        [Column("LoaiToken")]
        public CTokenType TokenType { get; set; }
        [Column("NgayHetHan")]
        public DateTimeOffset ExpiredDate { get; set; }
        [Column("DaThuHoi")]
        public bool IsRevoked { get; set; }
        [Column("NguoiDungId")]
        public Guid UserId { get; set; }
        [ForeignKey(name: nameof(UserTokenEntity.UserId))]
        [InverseProperty(property: nameof(UserEntity.UserTokens))]
        public virtual UserEntity User { get; set; } = null!;
    }
}