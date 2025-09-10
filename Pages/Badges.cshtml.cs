using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;
using MunicipalServicesApp.Data;

namespace MunicipalServicesApp.Pages
{
    public class BadgesModel : PageModel
    {
        private readonly DataService _dataService;
        private readonly IssuesRepository _issuesRepository;

        public BadgesModel(DataService dataService, IssuesRepository issuesRepository)
        {
            _dataService = dataService;
            _issuesRepository = issuesRepository;
        }

        public List<Badge> AllBadges { get; set; } = new List<Badge>();
        public List<Badge> EarnedBadges { get; set; } = new List<Badge>();
        public List<Badge> LockedBadges { get; set; } = new List<Badge>();
        public BadgeStats BadgeStats { get; set; } = new BadgeStats();
        public int TotalIssues { get; set; }
        public int TotalPoints { get; set; }

        public void OnGet()
        {
            // For demo purposes, use a default email - in real app, get from user session
            var demoEmail = "demo@municipal.gov.za";
            
            // Get user-specific badges
            EarnedBadges = _dataService.GetUserBadges(demoEmail);
            AllBadges = _issuesRepository.GetAllAvailableBadges();
            LockedBadges = AllBadges.Where(b => !EarnedBadges.Any(eb => eb.Id == b.Id)).ToList();
            BadgeStats = _dataService.GetUserBadgeStats(demoEmail);
            
            // Get issue statistics for this user
            var userIssues = _issuesRepository.GetAllIssues().Where(i => i.UserEmail == demoEmail).ToList();
            TotalIssues = userIssues.Count;
            TotalPoints = userIssues.Sum(i => i.UserPoints);
        }
    }
}
