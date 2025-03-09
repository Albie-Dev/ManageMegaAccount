using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MMA.Service
{
    public class UserLoginEntity : BaseEntity
    {
        [Column(name: "DiaChiIP")]
        [StringLength(maximumLength: 20)]
        public string IpAddress { get; set; } = string.Empty;
    }
}