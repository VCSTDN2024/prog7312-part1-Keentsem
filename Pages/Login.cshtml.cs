using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserRepository _userRepository;

        public LoginModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var user = _userRepository.LoginUser(Username, Password);
                
                if (user != null)
                {
                    // Store user info in session (simplified for portfolio)
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserLevel", user.Level.ToString());
                    HttpContext.Session.SetString("UserPoints", user.TotalPoints.ToString());
                    HttpContext.Session.SetString("UserMunicipality", user.Municipality);

                    TempData["SuccessMessage"] = $"Welcome back, {user.Username}! You are logged in as {user.Level} level with {user.TotalPoints} points.";
                    
                    return RedirectToPage("/Index");
                }
                else
                {
                    ErrorMessage = "Invalid username or password. Please try again.";
                    return Page();
                }
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred during login. Please try again.";
                return Page();
            }
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToPage("/Index");
        }

        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToPage("/Index");
        }
    }
}
