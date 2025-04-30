using System.ComponentModel.DataAnnotations;

namespace MMA.Domain
{
    public class BaseActorInfoDto
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "{0} không được để trống.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "{0} phải có ít nhất {2} ký tự và nhiều nhất {1} ký tự.")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Avatar không được để trống.")]
        [DataType(DataType.ImageUrl, ErrorMessage = "Avatar phải là URL hình ảnh hợp lệ.")]
        public string Avatar { get; set; } = string.Empty;
        [Required(ErrorMessage = "FileId không được để trống.")]
        public string FileId { get; set; } = string.Empty;
        [Required(ErrorMessage = "DateOfBirth không được để trống.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(BaseActorInfoDto), nameof(ValidatePastDate))]
        public DateOnly DateOfBirth { get; set; }
        [Required(ErrorMessage = "DebutDate không được để trống.")]
        [DataType(DataType.Date)]
        public DateOnly DebutDate { get; set; }
        [Range(30, 150, ErrorMessage = "Burt phải có giá trị từ {1} đến {2}.")]
        public int Bust { get; set; }
        [Range(30, 150, ErrorMessage = "Waist phải có giá trị từ {1} đến {2}.")]
        public int Waist { get; set; }
        [Range(30, 150, ErrorMessage = "Hips phải có giá trị từ {1} đến {2}.")]
        public int Hips { get; set; }
        public CCupSizeType CupSizeType { get; set; }
        public CRegionType RegionType { get; set; }
        [Range(80, 250, ErrorMessage = "Height phải có giá trị từ {1} đến {2}.")]
        public int Height { get; set; }
        [Required(ErrorMessage = "Address không được để trống.")]
        public string Address { get; set; } = string.Empty;
        [StringLength(1000, ErrorMessage = "Description không được vượt quá {1} ký tự.")]
        public string Description { get; set; } = string.Empty;
        public CMasterStatus Status { get; set; }
        public List<ActorInfoProperty> ActorInfos { get; set; } = new();

        public static ValidationResult? ValidatePastDate(DateOnly date, ValidationContext context)
        {
            if (date > DateOnly.FromDateTime(DateTime.Now))
            {
                return new ValidationResult($"{context.DisplayName} không được để ở tương lai.", new[] { context.MemberName ?? string.Empty });
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DebutDate < DateOfBirth)
            {
                yield return new ValidationResult("Ngày ra mắt phải sau hoặc bằng ngày sinh.", new[] { nameof(DebutDate) });
            }
        }
    }
}