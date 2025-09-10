using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesApp.Models
{
    public class Leaderboard
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public string UserName { get; set; } = string.Empty;
        
        public string UserEmail { get; set; } = string.Empty;
        
        public int TotalPoints { get; set; }
        
        public int ReportsSubmitted { get; set; }
        
        public int IssuesResolved { get; set; }
        
        public UserLevel Level { get; set; }
        
        public DateTime LastActiveDate { get; set; }
        
        public int Rank { get; set; }
        
        public List<Guid> BadgeIds { get; set; } = new List<Guid>();
        
        public string? Municipality { get; set; }
        
        public string? Province { get; set; }
    }

    public class MunicipalityStats
    {
        public string MunicipalityName { get; set; } = string.Empty;
        
        public string Province { get; set; } = string.Empty;
        
        public int TotalIssues { get; set; }
        
        public int ResolvedIssues { get; set; }
        
        public int ActiveUsers { get; set; }
        
        public double ResolutionRate => TotalIssues > 0 ? (double)ResolvedIssues / TotalIssues * 100 : 0;
        
        public List<IssueCategory> TopCategories { get; set; } = new List<IssueCategory>();
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }

    public class GamificationMetrics
    {
        public int TotalUsers { get; set; }
        
        public int TotalIssues { get; set; }
        
        public int TotalPointsAwarded { get; set; }
        
        public int TotalBadgesEarned { get; set; }
        
        public double AverageIssuesPerUser => TotalUsers > 0 ? (double)TotalIssues / TotalUsers : 0;
        
        public double AveragePointsPerUser => TotalUsers > 0 ? (double)TotalPointsAwarded / TotalUsers : 0;
        
        public List<UserLevel> LevelDistribution { get; set; } = new List<UserLevel>();
        
        public Dictionary<IssueCategory, int> CategoryDistribution { get; set; } = new Dictionary<IssueCategory, int>();
    }
}
