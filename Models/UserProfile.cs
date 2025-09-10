using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesApp.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Phone]
        public string? ContactNumber { get; set; }
        
        public string? Province { get; set; }
        
        public string? Municipality { get; set; }
        
        public int TotalPoints { get; set; } = 0;
        
        public int ReportsSubmitted { get; set; } = 0;
        
        public int IssuesResolved { get; set; } = 0;
        
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
        
        public DateTime LastActiveDate { get; set; } = DateTime.UtcNow;
        
        public List<Guid> EarnedBadges { get; set; } = new List<Guid>();
        
        public List<Guid> ReportedIssues { get; set; } = new List<Guid>();
        
        public UserLevel Level { get; set; } = UserLevel.Bronze;
        
        public int ExperiencePoints { get; set; } = 0;
    }

    public enum UserLevel
    {
        Bronze = 1,
        Silver = 2,
        Gold = 3,
        Platinum = 4,
        Diamond = 5
    }
}
