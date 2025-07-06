using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SahasBanking.Models.Account;
using SahasBanking.Models.BankController;
using SahasBanking.Models.Loan;

namespace SahasBanking.Models.AppIdentityDb
{
    public class AppIdentityDbContext : IdentityDbContext<Users>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }

        public DbSet<BankAccounts> BankAccounts { get; set; }
        public DbSet<LoanModel> Loans { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}

