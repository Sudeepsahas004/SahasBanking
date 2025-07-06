using System.ComponentModel.DataAnnotations;

namespace SahasBanking.Models.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Full Name is Mandatory")]
        public string? FullName { get; set; } = string.Empty;

        [EmailAddress]
        [Required(ErrorMessage = "Email is Mandatory")]
        public string? Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Provide secure password")]
        public string? Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password should match to previous one")]
        public string? ConfirmPassword { get; set; } = string.Empty;

        public string? Role { get; set; }
    }
}
