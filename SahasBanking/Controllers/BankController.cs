using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SahasBanking.Models.Account;
using SahasBanking.Models.AppIdentityDb;
using SahasBanking.Models.BankController;

namespace SahasBanking.Controllers
{
    public class BankController : Controller
    {
        private readonly AppIdentityDbContext _context;



        public BankController(AppIdentityDbContext context)

        {

            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Credit(string UserName)
        {

            var account = _context.BankAccounts.FirstOrDefault(u => u.UserName == UserName);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Credit(string Username, int AccountBalance)
        {

            var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.UserName == Username);
            if (account == null)
            {
                ViewBag.ErrorMessage = "Invalid User";
                return View("Error", "Bank");
            }

            if (AccountBalance <= 0)
            {

                ViewBag.ErrorMessage = "Amount must be greater than 0";
                return View("Error", "Bank");
            }

            var transaction = new Transaction
            {
                SenderUsername = null,
                ReceiverUsername = account.UserName,
                Amount = AccountBalance,
                Date = DateTime.Now,
                TransactionType = "Credit"
            };


            account.AccountBalance += AccountBalance;


            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            //ViewBag.Message = "Amount Credited Successfully";
            return RedirectToAction("MainPage", "Account");
        }

        public IActionResult Debit(string UserName)
        {

            var account = _context.BankAccounts.FirstOrDefault(u => u.UserName == UserName);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Debit(string Username, int AccountBalance)
        {
            var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.UserName == Username);
            if (account == null)
            {
                ViewBag.ErrorMessage = "Invalid User";
                return View("Error", "Bank");
            }


            if (AccountBalance <= 0)
            {
                ViewBag.ErrorMessage = "Amount must be greater than 0";
                return View("Error", "Bank");
            }

            if (account.AccountBalance < AccountBalance)
            {
                ViewBag.ErrorMessage = "Insufficient Funds";
                return View("Error", "Bank");
            }
            var transaction = new Transaction
            {
                SenderUsername = account.UserName,
                ReceiverUsername = null,
                Amount = AccountBalance,
                Date = DateTime.Now,
                TransactionType = "Debit"
            };
            account.AccountBalance -= AccountBalance;
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();

            //ViewBag.Message = "Amount Credited Successfully";
            return RedirectToAction("MainPage", "Account");

        }

        public IActionResult Transactions(string UserName)
        {
            var transactions = _context.Transactions
                .Where(t => t.SenderUsername == UserName || t.ReceiverUsername == UserName)
                .OrderByDescending(t => t.Date)
                .ToList();

            return View(transactions);
        }

        public async Task<IActionResult> CheckBalance(string UserName)
        {
            var account = await _context.BankAccounts.FirstOrDefaultAsync(a => a.UserName == UserName);
            if (account == null)
            {
                return NotFound();
            }


            return View(account);
        }


        public IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View();
        }



    }
}


