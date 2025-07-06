using System.ComponentModel.DataAnnotations;

namespace SahasBanking.Models.BankController
{
    public class BankAccounts
    {
        [Key]
        public int Id { get; set; }
        
        public int AccountBalance { get; set; }

        public string? UserName { get; set; } = string.Empty;
    }
}
