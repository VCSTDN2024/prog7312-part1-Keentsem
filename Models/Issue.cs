




using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesApp.Models
{
    public partial class Issue : IMunicipalEntity, IMunicipalComparable, IMunicipalWorkflow
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Location is required")]
        [StringLength(300, ErrorMessage = "Location cannot exceed 300 characters")]
        public string Location { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Category is required")]
        public IssueCategory Category { get; set; }
        
        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;
        
        public IssuePriority Priority { get; set; } = IssuePriority.Medium;
        
        public IssueStatus Status { get; set; } = IssueStatus.Open;
        
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ResolvedDate { get; set; }
        
        public List<string> AttachmentPaths { get; set; } = new List<string>();
        
        public int UserPoints { get; set; } = 10; // Points awarded for reporting
        
        // Public property for easier access to points
        public int Points => UserPoints;
        
        public string? UserEmail { get; set; }
        
        public string? ContactNumber { get; set; }
        
        // IMunicipalEntity implementation
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModified { get; set; }
        public string Municipality { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        
        // IMunicipalComparable explicit implementation
        int IMunicipalComparable.Priority => (int)this.Priority;
        DateTime IMunicipalComparable.SubmittedAt => this.SubmittedAt;
        int IMunicipalComparable.Points => this.UserPoints;
        
        // IMunicipalWorkflow implementation
        public bool CanMerge => Status == IssueStatus.Open || Status == IssueStatus.InProgress;
        public bool CanEscalate => Priority != IssuePriority.Critical && Status == IssueStatus.Open;
        public bool CanAssign => Status == IssueStatus.Open;
        public string WorkflowStatus => Status.ToString();
        
        // IMunicipalComparable implementation
        public int CompareTo(IMunicipalComparable? other)
        {
            if (other == null) return 1;
            
            // Primary sort: Priority (higher priority first)
            int priorityComparison = other.Priority.CompareTo(this.Priority);
            if (priorityComparison != 0) return priorityComparison;
            
            // Secondary sort: Points (higher points first)
            int pointsComparison = other.Points.CompareTo(this.Points);
            if (pointsComparison != 0) return pointsComparison;
            
            // Tertiary sort: Date (newer first)
            return other.SubmittedAt.CompareTo(this.SubmittedAt);
        }
    }

    public enum IssueCategory
    {
        WaterSupply,
        Electricity,
        Roads,
        WasteManagement,
        PublicSafety,
        ParksAndRecreation,
        BuildingPermits,
        Other
    }

    public enum IssuePriority
    {
        Low = 1,
        Medium = 2,
        High = 3,
        Critical = 4
    }

    public enum IssueStatus
    {
        Open,
        InProgress,
        Resolved,
        Closed
    }

    // Learning Unit 2: Operator Overloading Implementation
    public partial class Issue : IComparable<Issue>
    {
        /// <summary>
        /// Implements IComparable for sorting and comparison operations
        /// </summary>
        public int CompareTo(Issue? other)
        {
            if (other == null) return 1;
            
            // Primary sort: Priority (higher priority first)
            int priorityComparison = other.Priority.CompareTo(this.Priority);
            if (priorityComparison != 0) return priorityComparison;
            
            // Secondary sort: Points (higher points first)
            int pointsComparison = other.UserPoints.CompareTo(this.UserPoints);
            if (pointsComparison != 0) return pointsComparison;
            
            // Tertiary sort: Date (newer first)
            return other.SubmittedAt.CompareTo(this.SubmittedAt);
        }

        /// <summary>
        /// Operator overloading for greater than comparison
        /// </summary>
        public static bool operator >(Issue? issue1, Issue? issue2)
        {
            if (issue1 == null) return false;
            if (issue2 == null) return true;
            return issue1.CompareTo(issue2) > 0;
        }

        /// <summary>
        /// Operator overloading for less than comparison
        /// </summary>
        public static bool operator <(Issue? issue1, Issue? issue2)
        {
            if (issue1 == null) return issue2 != null;
            if (issue2 == null) return false;
            return issue1.CompareTo(issue2) < 0;
        }

        /// <summary>
        /// Operator overloading for greater than or equal comparison
        /// </summary>
        public static bool operator >=(Issue? issue1, Issue? issue2)
        {
            if (issue1 == null) return issue2 == null;
            if (issue2 == null) return true;
            return issue1.CompareTo(issue2) >= 0;
        }

        /// <summary>
        /// Operator overloading for less than or equal comparison
        /// </summary>
        public static bool operator <=(Issue? issue1, Issue? issue2)
        {
            if (issue1 == null) return true;
            if (issue2 == null) return false;
            return issue1.CompareTo(issue2) <= 0;
        }

        /// <summary>
        /// Operator overloading for addition (merge two issues)
        /// </summary>
        public static Issue operator +(Issue? issue1, Issue? issue2)
        {
            if (issue1 == null || issue2 == null)
                throw new ArgumentNullException("Cannot merge null issues");

            return new Issue
            {
                Title = $"{issue1.Title} + {issue2.Title}",
                Description = $"{issue1.Description}\n\nMerged with: {issue2.Description}",
                Category = issue1.Category,
                Priority = (issue1.Priority > issue2.Priority) ? issue1.Priority : issue2.Priority,
                Location = issue1.Location,
                UserPoints = issue1.UserPoints + issue2.UserPoints,
                UserEmail = issue1.UserEmail,
                ContactNumber = issue1.ContactNumber ?? issue2.ContactNumber,
                AttachmentPaths = issue1.AttachmentPaths.Concat(issue2.AttachmentPaths).ToList()
            };
        }

        /// <summary>
        /// Operator overloading for multiplication (scale points)
        /// </summary>
        public static Issue operator *(Issue? issue, int multiplier)
        {
            if (issue == null)
                throw new ArgumentNullException("Cannot scale null issue");

            return new Issue
            {
                Id = issue.Id,
                Title = issue.Title,
                Description = issue.Description,
                Category = issue.Category,
                Priority = issue.Priority,
                Location = issue.Location,
                UserPoints = issue.UserPoints * multiplier,
                UserEmail = issue.UserEmail,
                ContactNumber = issue.ContactNumber,
                AttachmentPaths = new List<string>(issue.AttachmentPaths),
                SubmittedAt = issue.SubmittedAt,
                ResolvedDate = issue.ResolvedDate,
                Status = issue.Status
            };
        }

        /// <summary>
        /// Operator overloading for equality comparison
        /// </summary>
        public static bool operator ==(Issue? issue1, Issue? issue2)
        {
            if (ReferenceEquals(issue1, issue2)) return true;
            if (issue1 is null || issue2 is null) return false;
            return issue1.Id == issue2.Id;
        }

        /// <summary>
        /// Operator overloading for inequality comparison
        /// </summary>
        public static bool operator !=(Issue? issue1, Issue? issue2)
        {
            return !(issue1 == issue2);
        }

        /// <summary>
        /// Override Equals method
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is Issue other)
                return this == other;
            return false;
        }

        /// <summary>
        /// Override GetHashCode method
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
