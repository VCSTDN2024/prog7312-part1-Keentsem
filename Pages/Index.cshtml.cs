using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MunicipalServicesApp.Services;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Data;

namespace MunicipalServicesApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IssuesRepository _issuesRepository;
        private readonly UserSessionService _userSessionService;

        public IndexModel(ILogger<IndexModel> logger, IssuesRepository issuesRepository, UserSessionService userSessionService)
        {
            _logger = logger;
            _issuesRepository = issuesRepository;
            _userSessionService = userSessionService;
        }

        public int TotalIssues { get; set; }
        public int ResolvedIssues { get; set; }
        public int OpenIssues { get; set; }
        public int TotalPoints { get; set; }
        public List<Issue> RecentIssues { get; set; } = new List<Issue>();
        public UserSessionInfo UserInfo { get; set; } = new UserSessionInfo();

        public void OnGet()
        {
            TotalIssues = _issuesRepository.GetTotalIssuesCount();
            ResolvedIssues = _issuesRepository.GetResolvedIssuesCount();
            OpenIssues = _issuesRepository.GetIssuesByStatus(IssueStatus.Open).Count();
            TotalPoints = _issuesRepository.GetTotalUserPoints();
            RecentIssues = _issuesRepository.GetAllIssues().Take(6).ToList();
            
            // Get user session info
            UserInfo = _userSessionService.GetUserSessionInfo(HttpContext);
            
            // Set ViewData for layout
            ViewData["TotalPoints"] = UserInfo.TotalPoints;
            ViewData["IsLoggedIn"] = UserInfo.IsLoggedIn;
            ViewData["Username"] = UserInfo.Username;
            ViewData["UserEmail"] = UserInfo.Email;
            ViewData["UserLevel"] = UserInfo.Level.ToString();
            ViewData["UserMunicipality"] = UserInfo.Municipality;
            ViewData["ReportsSubmitted"] = UserInfo.ReportsSubmitted;
            ViewData["EarnedBadgesCount"] = UserInfo.EarnedBadgesCount;
        }
    }
}
