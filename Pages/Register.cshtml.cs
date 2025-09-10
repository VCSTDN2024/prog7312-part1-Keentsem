using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserRepository _userRepository;

        public RegisterModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [BindProperty]
        public string Username { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string ConfirmPassword { get; set; } = string.Empty;

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        public string Municipality { get; set; } = string.Empty;

        [BindProperty]
        public string Province { get; set; } = string.Empty;

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

            // Validate password confirmation
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return Page();
            }

            try
            {
                var newUser = _userRepository.RegisterUser(
                    Username, 
                    Password, 
                    Email, 
                    FullName, 
                    Municipality, 
                    Province
                );
                
                if (newUser != null)
                {
                    TempData["SuccessMessage"] = $"Welcome to the South African Municipal Services Portal, {newUser.Username}! Your account has been created successfully. You can now login to start earning badges and reporting issues.";
                    return RedirectToPage("/Login");
                }
                else
                {
                    ErrorMessage = "Username already exists. Please choose a different username.";
                    return Page();
                }
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred during registration. Please try again.";
                return Page();
            }
        }
    }
}
