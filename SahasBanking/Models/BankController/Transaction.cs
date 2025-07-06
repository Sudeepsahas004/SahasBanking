using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SahasBanking.Models.BankController
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

     
       
        public string? SenderUsername { get; set; }= string.Empty;  // Changed from SenderId
        public string? ReceiverUsername { get; set; } = string.Empty; // Changed from ReceiverId

        public string? TransactionType { get; set; } = string.Empty;


        public int Amount { get; set; }
        public DateTime Date { get; set; }


    }
}
