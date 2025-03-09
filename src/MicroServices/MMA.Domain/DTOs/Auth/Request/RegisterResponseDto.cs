using System.ComponentModel.DataAnnotations;

namespace MMA.Domain
{
    public class RegisterRequestDto
    {
        [StringLength(maximumLength: 100, MinimumLength = 8, ErrorMessage = "{0} phải có ít nhất {1} ký tự và nhiều nhất {2} ký tự.")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        [StringLength(maximumLength: 100, MinimumLength = 8, ErrorMessage = "{0} phải có ít nhất {1} ký tự và nhiều nhất {2} ký tự.")]
        [Required(ErrorMessage = "{0} không được để trống.")]
        public string FullName { get; set; } = string.Empty;
    }
}