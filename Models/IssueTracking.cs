using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesApp.Models
{
    public class IssueTracking
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid IssueId { get; set; }
        
        public IssueStatus PreviousStatus { get; set; }
        
        public IssueStatus NewStatus { get; set; }
        
        public DateTime StatusChangedAt { get; set; } = DateTime.UtcNow;
        
        public string? ChangedBy { get; set; }
        
        public string? Comments { get; set; }
        
        public bool IsPublicUpdate { get; set; } = true;
        
        public List<string> Attachments { get; set; } = new List<string>();
    }

    public class IssueComment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        public Guid IssueId { get; set; }
        
        [Required]
        [StringLength(500)]
        public string Comment { get; set; } = string.Empty;
        
        public string? AuthorName { get; set; }
        
        public string? AuthorEmail { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public bool IsInternal { get; set; } = false;
        
        public CommentType Type { get; set; } = CommentType.General;
    }

    public enum CommentType
    {
        General,
        StatusUpdate,
        Resolution,
        FollowUp,
        Escalation
    }
}
