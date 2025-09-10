using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace MunicipalServicesApp.Models
{
    /// <summary>
    /// Represents a user in the Municipal Services Portal system.
    /// Stores user credentials, earned badges, and reported issues for gamification tracking.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username for login authentication
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for login authentication (plain text for portfolio demonstration)
        /// Note: In production, this should be hashed using ASP.NET Identity or similar
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// ArrayList of earned badge IDs/names for gamification tracking
        /// Using ArrayList for dynamic, non-generic storage as requested
        /// </summary>
        public ArrayList Badges { get; set; } = new ArrayList();

        /// <summary>
        /// ArrayList of Issue objects that this user has reported
        /// Using ArrayList for maximum flexibility and demonstration purposes
        /// </summary>
        public ArrayList ReportedIssues { get; set; } = new ArrayList();

        /// <summary>
        /// User's email address for contact and notifications
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        /// <summary>
        /// User's full name for display purposes
        /// </summary>
        [StringLength(100, ErrorMessage = "Full name cannot exceed 100 characters")]
        public string? FullName { get; set; }

        /// <summary>
        /// User's municipality for location-based gamification
        /// </summary>
        [StringLength(100, ErrorMessage = "Municipality cannot exceed 100 characters")]
        public string? Municipality { get; set; }

        /// <summary>
        /// User's province for regional statistics
        /// </summary>
        [StringLength(50, ErrorMessage = "Province cannot exceed 50 characters")]
        public string? Province { get; set; }

        /// <summary>
        /// Total points earned by the user through reporting issues and earning badges
        /// </summary>
        public int TotalPoints { get; set; } = 0;

        /// <summary>
        /// Date and time when the user account was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date and time when the user was last active
        /// </summary>
        public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// User's current level based on total points (Bronze, Silver, Gold, Platinum, Diamond)
        /// </summary>
        public UserLevel Level { get; set; } = UserLevel.Bronze;

        /// <summary>
        /// Whether the user account is active and can log in
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Default constructor for creating new users
        /// </summary>
        public User()
        {
            Badges = new ArrayList();
            ReportedIssues = new ArrayList();
        }

        /// <summary>
        /// Constructor for creating a user with basic information
        /// </summary>
        /// <param name="username">The username for login</param>
        /// <param name="password">The password for login</param>
        /// <param name="email">The user's email address</param>
        public User(string username, string password, string email = "")
        {
            Username = username;
            Password = password;
            Email = email;
            Badges = new ArrayList();
            ReportedIssues = new ArrayList();
            CreatedAt = DateTime.UtcNow;
            LastActiveAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Adds a badge to the user's collection
        /// </summary>
        /// <param name="badgeId">The ID or name of the badge to add</param>
        public void AddBadge(string badgeId)
        {
            if (!Badges.Contains(badgeId))
            {
                Badges.Add(badgeId);
            }
        }

        /// <summary>
        /// Adds an issue to the user's reported issues collection
        /// </summary>
        /// <param name="issue">The Issue object to add</param>
        public void AddReportedIssue(Issue issue)
        {
            if (issue != null && !ReportedIssues.Contains(issue))
            {
                ReportedIssues.Add(issue);
                TotalPoints += issue.UserPoints;
                UpdateUserLevel();
            }
        }

        /// <summary>
        /// Updates the user's level based on total points
        /// </summary>
        private void UpdateUserLevel()
        {
            Level = TotalPoints switch
            {
                >= 1000 => UserLevel.Diamond,
                >= 500 => UserLevel.Platinum,
                >= 250 => UserLevel.Gold,
                >= 100 => UserLevel.Silver,
                _ => UserLevel.Bronze
            };
        }

        /// <summary>
        /// Checks if the user has earned a specific badge
        /// </summary>
        /// <param name="badgeId">The badge ID to check for</param>
        /// <returns>True if the user has the badge, false otherwise</returns>
        public bool HasBadge(string badgeId)
        {
            return Badges.Contains(badgeId);
        }

        /// <summary>
        /// Gets the count of reported issues by category
        /// </summary>
        /// <param name="category">The issue category to count</param>
        /// <returns>The number of issues in the specified category</returns>
        public int GetIssueCountByCategory(IssueCategory category)
        {
            int count = 0;
            foreach (Issue issue in ReportedIssues)
            {
                if (issue.Category == category)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Updates the last active timestamp
        /// </summary>
        public void UpdateLastActive()
        {
            LastActiveAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Returns a string representation of the user
        /// </summary>
        /// <returns>User information as a formatted string</returns>
        public override string ToString()
        {
            return $"User: {Username} ({FullName ?? "No Name"}) - Level: {Level} - Points: {TotalPoints} - Badges: {Badges.Count} - Issues: {ReportedIssues.Count}";
        }
    }

}
