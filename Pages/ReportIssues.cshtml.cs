using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Pages
{
    public class ReportIssuesModel : PageModel
    {
        private readonly IssueManagementService _issueManagementService;
        private readonly NotificationService _notificationService;
        private readonly UserSessionService _userSessionService;

        public ReportIssuesModel(IssueManagementService issueManagementService, NotificationService notificationService, UserSessionService userSessionService)
        {
            _issueManagementService = issueManagementService;
            _notificationService = notificationService;
            _userSessionService = userSessionService;
        }

        // This property is bound to the form on ReportIssues.cshtml
        [BindProperty]
        public Issue Issue { get; set; } = new Issue();

        [BindProperty]
        public List<IFormFile> MediaFiles { get; set; } = new List<IFormFile>();

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public List<Notification> RecentNotifications { get; set; } = new List<Notification>();
        public List<Badge> NewBadges { get; set; } = new List<Badge>();

        public void OnGet(string? category = null)
        {
            if (!string.IsNullOrEmpty(category) && Enum.TryParse<IssueCategory>(category, out var categoryEnum))
            {
                Issue.Category = categoryEnum;
            }
            
            // Get current user info and set ViewData
            var userInfo = _userSessionService.GetUserSessionInfo(HttpContext);
            var userEmail = _userSessionService.GetCurrentUserEmail(HttpContext);
            
            // Set ViewData for layout
            ViewData["TotalPoints"] = userInfo.TotalPoints;
            ViewData["IsLoggedIn"] = userInfo.IsLoggedIn;
            ViewData["Username"] = userInfo.Username;
            ViewData["UserEmail"] = userInfo.Email;
            ViewData["UserLevel"] = userInfo.Level.ToString();
            ViewData["UserMunicipality"] = userInfo.Municipality;
            ViewData["ReportsSubmitted"] = userInfo.ReportsSubmitted;
            ViewData["EarnedBadgesCount"] = userInfo.EarnedBadgesCount;
            
            // Get recent notifications for display
            RecentNotifications = _notificationService.GetRecentNotifications(userEmail, 3);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                // Get current user email from session
                var userEmail = _userSessionService.GetCurrentUserEmail(HttpContext);
                Issue.UserEmail = userEmail;

                // Handle file uploads
                if (MediaFiles != null && MediaFiles.Any())
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    foreach (var file in MediaFiles.Take(5)) // Limit to 5 files
                    {
                        if (file.Length > 0)
                        {
                            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                            var filePath = Path.Combine(uploadsFolder, fileName);
                            
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            
                            Issue.AttachmentPaths.Add($"/uploads/{fileName}");
                        }
                    }
                }

                // Process the issue through the new IssueManagementService
                var result = await _issueManagementService.SubmitIssueAsync(Issue);
                
                if (result.Success)
                {
                    SuccessMessage = $"Thank you for your report! You've earned {result.PointsAwarded} community points. Your issue has been submitted and will be reviewed by our team.";
                    
                    // Store new badges and notifications for display
                    NewBadges = result.BadgesEarned;
                    RecentNotifications = result.NotificationsCreated;
                    
                    // Refresh user info and ViewData
                    var userInfo = _userSessionService.GetUserSessionInfo(HttpContext);
                    ViewData["TotalPoints"] = userInfo.TotalPoints;
                    ViewData["IsLoggedIn"] = userInfo.IsLoggedIn;
                    ViewData["Username"] = userInfo.Username;
                    ViewData["UserEmail"] = userInfo.Email;
                    ViewData["UserLevel"] = userInfo.Level.ToString();
                    ViewData["UserMunicipality"] = userInfo.Municipality;
                    ViewData["ReportsSubmitted"] = userInfo.ReportsSubmitted;
                    ViewData["EarnedBadgesCount"] = userInfo.EarnedBadgesCount;
                    
                    // Get updated notifications
                    RecentNotifications = _notificationService.GetRecentNotifications(userEmail, 3);
                    
                    // Reset the form
                    Issue = new Issue();
                    MediaFiles = new List<IFormFile>();
                    ModelState.Clear();
                }
                else
                {
                    ErrorMessage = result.ErrorMessage ?? "An error occurred while submitting your report. Please try again.";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred while submitting your report. Please try again.";
                // Log the exception in a real application
            }

            return Page();
        }
    }
}
