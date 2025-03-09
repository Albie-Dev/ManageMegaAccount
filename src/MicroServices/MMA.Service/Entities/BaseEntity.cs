using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.Service
{
    public abstract class BaseEntity : BaseInfo
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }

    public abstract class BaseInfo
    {
        [Column(name: "NguoiTaoId")]
        public Guid CreatedBy { get; set; }
        [Column(name: "NguoiSuaDoiId")]
        public Guid ModifiedBy { get; set; }
        private DateTimeOffset _createdDate = DateTimeOffset.UtcNow;
        [Column(name: "NgayTao")]
        public DateTimeOffset CreatedDate
        {
            get => _createdDate.ToLocalTime();
            private set => _createdDate = value;
        }

        private DateTimeOffset _modifiedDate = DateTimeOffset.UtcNow;
        [Column(name: "NgaySuaDoi")]
        public DateTimeOffset ModifiedDate
        {
            get => _modifiedDate.ToLocalTime();
            set => _modifiedDate = value;
        }
    }
}