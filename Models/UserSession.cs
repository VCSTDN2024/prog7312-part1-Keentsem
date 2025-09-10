using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesApp.Models
{
    public class UserSession : IMunicipalEntity
    {
        // IMunicipalEntity implementation
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModified { get; set; }
        public string Municipality { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        // UserSession specific properties
        public Guid SessionId { get; set; } = Guid.NewGuid();
        
        [Required]
        public string UserEmail { get; set; } = string.Empty;
        
        public string UserName { get; set; } = string.Empty;
        
        public int TotalPoints { get; set; } = 0;
        
        public int ReportsSubmitted { get; set; } = 0;
        
        public List<Guid> ReportedIssues { get; set; } = new List<Guid>();
        
        public List<int> EarnedBadges { get; set; } = new List<int>();
        
        public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
        
        public UserLevel Level { get; set; } = UserLevel.Bronze;
    }
}
