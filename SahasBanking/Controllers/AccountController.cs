// AccountController.cs
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SahasBanking.Models.Account;
using SahasBanking.Models.AppIdentityDb;
using SahasBanking.Models.BankController;
using System.Security.Claims;

namespace SahasBanking.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;
        private readonly AppIdentityDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<Users> userManager, SignInManager<Users> signInManager, AppIdentityDbContext context,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (await _userManager.FindByNameAsync(model.FullName) != null)
                {
                    ModelState.AddModelError("UserName", "The username is already taken.");
                    return View(model);
                }

                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("Email", "The email is already in use.");
                    return View(model);
                }

                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                    return View(model);
                }

                var user = new Users
                {
                    UserName = model.FullName,
                    Email = model.Email,
                    CustomEmail = model.Email
                };

               
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    }
                    if (!await _roleManager.RoleExistsAsync("User"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    //assign the role
                    string role = model.Role ?? "User"; //Default
                    if (role == "User")
                    {
                        var account = new BankAccounts
                        { UserName = model.FullName, AccountBalance = 0 };
                        _context.BankAccounts.Add(account);
                        _context.SaveChanges();
                    }
                    if (role == "Admin" && User.IsInRole("Admin"))
                    {
                        ModelState.AddModelError("", "Only for Admin");
                        return View(model);
                    }
                    await _userManager.AddToRoleAsync(user, role);
                    var claims = new List<Claim>
                     {
                         new Claim(ClaimTypes.Name, model.FullName),
                         new Claim(ClaimTypes.Email,model.Email),
                         new Claim(ClaimTypes.Role,"User") // custom role default im taking as user...
 
                     };
                    await _userManager.AddClaimsAsync(user, claims);

                    return RedirectToAction("Login", "Account");
                }

               


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public IActionResult Login()   
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt. Username does not exist.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, isPersistent: true, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                // Get user claims
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                // Create Claims Identity
                //Represents the identity of the user
                var claimsIdentity = new ClaimsIdentity(userClaims, "ApplicationCookie");

                //adds the claim representing the username
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name,model.UserName));
                foreach (var role in roles)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                //represents the authenticated user
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign in with claims
               await HttpContext.SignInAsync(claimsPrincipal);


                return RedirectToAction("MainPage", "Account");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt. Incorrect password.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> MainPage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                return View(user);
            }

            return RedirectToAction("Login", "Account");
        }
    }
}
