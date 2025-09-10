using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesApp.Models
{
    public class Badge : IComparable<Badge>
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string ImagePath { get; set; } = string.Empty;
        
        public BadgeType Type { get; set; }
        
        public int RequiredCount { get; set; }
        
        public string RequiredCategory { get; set; } = string.Empty;
        
        public bool IsEarned { get; set; } = false;
        
        public DateTime? EarnedDate { get; set; }
        
        public int PointsValue { get; set; } = 0;

        /// <summary>
        /// Implements IComparable for sorting and comparison operations
        /// </summary>
        public int CompareTo(Badge? other)
        {
            if (other == null) return 1;
            
            // Primary sort: Points value (higher points first)
            int pointsComparison = other.PointsValue.CompareTo(this.PointsValue);
            if (pointsComparison != 0) return pointsComparison;
            
            // Secondary sort: Earned date (earlier earned first)
            if (this.EarnedDate.HasValue && other.EarnedDate.HasValue)
            {
                int dateComparison = this.EarnedDate.Value.CompareTo(other.EarnedDate.Value);
                if (dateComparison != 0) return dateComparison;
            }
            else if (this.EarnedDate.HasValue && !other.EarnedDate.HasValue)
            {
                return -1; // This is earned, other is not
            }
            else if (!this.EarnedDate.HasValue && other.EarnedDate.HasValue)
            {
                return 1; // Other is earned, this is not
            }
            
            // Tertiary sort: Name (alphabetical)
            return this.Name.CompareTo(other.Name);
        }
    }

    public enum BadgeType
    {
        FirstReport,
        CommunityHelper,
        CategorySpecialist,
        MediaContributor,
        PriorityReporter,
        ConsistentReporter,
        EmergencyResponder,
        CommunityChampion,
        MunicipalAdvocate,
        ServiceExcellence
    }
}
