using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SahasBanking.Models.Account
{
    public class Users : IdentityUser
    {
        [Key]
        public int CustomId { get; set; } // Renamed to avoid conflict with IdentityUser's Id
        public string? CustomEmail { get; set; } = string.Empty; // Renamed to avoid conflict with IdentityUser's Email
        public string? CustomPassword { get; set; } = string.Empty; // Renamed to avoid conflict with IdentityUser's Password
    }
}
