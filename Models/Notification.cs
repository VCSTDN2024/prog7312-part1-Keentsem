using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesApp.Models
{
    /// <summary>
    /// Notification model for user feedback and system alerts
    /// </summary>
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required]
        public string UserEmail { get; set; } = string.Empty;
        
        [Required]
        public NotificationType Type { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        public string? ImagePath { get; set; }
        
        public int? PointsAwarded { get; set; }
        
        public Guid? RelatedIssueId { get; set; }
        
        public int? RelatedBadgeId { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ReadAt { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public Issue? RelatedIssue { get; set; }
        public Badge? RelatedBadge { get; set; }
    }

    public enum NotificationType
    {
        BadgeEarned,
        IssueSubmitted,
        IssueStatusChanged,
        IssueResolved,
        PointsAwarded,
        SystemAlert,
        AchievementUnlocked,
        MilestoneReached
    }

    /// <summary>
    /// Badge earning notification with enhanced details
    /// </summary>
    public class BadgeEarningNotification
    {
        public Badge Badge { get; set; } = new Badge();
        public int PointsAwarded { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
        public string AchievementMessage { get; set; } = string.Empty;
        public bool IsNewBadge { get; set; } = true;
    }

    /// <summary>
    /// Notification statistics for dashboard
    /// </summary>
    public class NotificationStats
    {
        public int TotalNotifications { get; set; }
        public int UnreadNotifications { get; set; }
        public int BadgeNotifications { get; set; }
        public int IssueNotifications { get; set; }
        public int SystemNotifications { get; set; }
        public DateTime LastNotificationDate { get; set; }
    }
}
