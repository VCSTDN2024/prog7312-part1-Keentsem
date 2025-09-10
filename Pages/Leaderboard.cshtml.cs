using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Data;

namespace MunicipalServicesApp.Pages
{
    public class LeaderboardModel : PageModel
    {
        private readonly IssuesRepository _repository;

        public LeaderboardModel(IssuesRepository repository)
        {
            _repository = repository;
        }

        public List<Leaderboard> Leaderboard { get; set; } = new List<Leaderboard>();
        public GamificationMetrics Metrics { get; set; } = new GamificationMetrics();
        public List<MunicipalityStats> MunicipalityStats { get; set; } = new List<MunicipalityStats>();
        public Dictionary<IssueCategory, int> CategoryStats { get; set; } = new Dictionary<IssueCategory, int>();

        public void OnGet()
        {
            Leaderboard = _repository.GetLeaderboard(20);
            Metrics = _repository.GetGamificationMetrics();
            CategoryStats = _repository.GetCategoryStatistics();
            
            // Get top municipalities
            var municipalityStats = _repository.GetMunicipalityStatistics();
            MunicipalityStats = municipalityStats
                .OrderByDescending(kvp => kvp.Value)
                .Take(6)
                .Select(kvp => _repository.GetMunicipalityStats(kvp.Key))
                .ToList();
        }
    }
}
