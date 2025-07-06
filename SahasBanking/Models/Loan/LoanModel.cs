namespace SahasBanking.Models.Loan
{
    public class LoanModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } // Loan taker (username)
        public double Amount { get; set; } // Loan amount
        public LoanStatus Status { get; set; } = LoanStatus.Pending; // Admin approval status


        public enum LoanStatus
        {
            Pending,
            Approved,
            Rejected
        }

    } }
