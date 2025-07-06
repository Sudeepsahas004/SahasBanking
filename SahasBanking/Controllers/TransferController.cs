using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SahasBanking.Models.AppIdentityDb;
using SahasBanking.Models.BankController;
using SahasBanking.Models.Transfer;
using SahasBanking.Models.Account;
using System.Security.Claims;

namespace SahasBanking.Controllers
{
    [Authorize]
    public class TransferController : Controller
    {
        private readonly AppIdentityDbContext _context;
        private readonly UserManager<Users> _userManager;

        public TransferController(AppIdentityDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transfer/AllAccounts
        // GET: Transfer/SendTo
        public async Task<IActionResult> SendTo()
        {
            var currentUsername = User.Identity.Name;

            var accounts = await _context.BankAccounts
                .Where(a => a.UserName != currentUsername) // Exclude current user
                .ToListAsync();

            return View(accounts);
        }


        // GET: Transfer/TransferMoney?username=ReceiverUser
        [HttpGet]
        public IActionResult TransferMoney(string username)
        {
            var model = new TransferViewModel
            {
                ReceiverUsername = username
            };
            return View(model);
        }

        // POST: Transfer/TransferMoney
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferMoney(TransferViewModel model)
        {
            var senderUsername = User.Identity.Name;

            if (senderUsername == model.ReceiverUsername)
            {
                ModelState.AddModelError("", "You cannot transfer money to yourself.");
                return View(model);
            }

            var senderAccount = await _context.BankAccounts.FirstOrDefaultAsync(a => a.UserName == senderUsername);
            var receiverAccount = await _context.BankAccounts.FirstOrDefaultAsync(a => a.UserName == model.ReceiverUsername);

            if (senderAccount == null || receiverAccount == null)
            {
                ModelState.AddModelError("", "Invalid sender or receiver account.");
                return View(model);
            }

            if (model.Amount <= 0)
            {
                ModelState.AddModelError("", "Transfer amount must be greater than zero.");
                return View(model);
            }

            if (senderAccount.AccountBalance < model.Amount)
            {
                ModelState.AddModelError("", "Insufficient balance.");
                return View(model);
            }

            // Perform the transfer
            senderAccount.AccountBalance -= model.Amount;
            receiverAccount.AccountBalance += model.Amount;

            // Log transaction
            var transaction = new Transaction
            {
                SenderUsername = senderAccount.UserName,
                ReceiverUsername = receiverAccount.UserName,
                TransactionType = "Transfer",
                Amount = model.Amount,
                Date = DateTime.Now
            };
            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            ViewBag.ReceiverName = model.ReceiverUsername;
            return View("Success");
        }
    }
}
