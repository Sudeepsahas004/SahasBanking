using System.ComponentModel.DataAnnotations;

namespace SahasBanking.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Enter Username to login")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        public bool RememberMe { get; internal set; }
    }
}
