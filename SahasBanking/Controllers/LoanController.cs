using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SahasBanking.Models.AppIdentityDb;
using SahasBanking.Models.Loan;
using System.Security.Claims;

namespace SahasBanking.Controllers
{
    [Authorize]
    public class LoanController : Controller
    {
        private readonly AppIdentityDbContext _context;

        public LoanController(AppIdentityDbContext context)
        {
            _context = context;
        }

        // GET: Loan/Apply
        [HttpGet]
        public IActionResult Apply()
        {
            return View();
        }

        // POST: Loan/Apply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(double amount)
        {
            var username = User.Identity.Name;

            var newLoan = new LoanModel
            {
                UserName = username,
                Amount = amount,
                Status = LoanModel.LoanStatus.Pending
            };

            _context.Loans.Add(newLoan);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyLoans");
        }

        // GET: Loan/MyLoans
        [HttpGet]
        public async Task<IActionResult> MyLoans()
        {
            var username = User.Identity.Name;

            var loans = await _context.Loans
                .Where(l => l.UserName == username)
                .ToListAsync();

            return View(loans);
        }

        // GET: Loan/Requests
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LoanRequests()
        {
            var pendingLoans = await _context.Loans
                .Where(l => l.Status == LoanModel.LoanStatus.Pending)
                .ToListAsync();

            if (!pendingLoans.Any())
            {
                return View("NoRequests");
            }

            return View(pendingLoans);
        }

        // GET: Loan/Approve?username=xyz
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(string username)
        {
            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.UserName == username && l.Status == LoanModel.LoanStatus.Pending);

            if (loan != null)
            {
                loan.Status = LoanModel.LoanStatus.Approved;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("NoRequests");
        }

        // GET: Loan/Reject?username=xyz
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(string username)
        {
            var loan = await _context.Loans
                .FirstOrDefaultAsync(l => l.UserName == username && l.Status == LoanModel.LoanStatus.Pending);

            if (loan != null)
            {
                loan.Status = LoanModel.LoanStatus.Rejected;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("NoRequests");
        }
    }
}
