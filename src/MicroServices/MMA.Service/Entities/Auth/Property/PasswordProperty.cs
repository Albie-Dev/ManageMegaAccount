using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.Service
{
    public class PasswordProperty
    {
        [Column("MatKhauMaHoa")]
        public string PasswordHash { get; set; } = string.Empty;
        [Column("NgayDoiMatKhau")]
        public DateTimeOffset ChangedDate { get; set; }
    }
}