using System.Collections.Generic;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    public static class MunicipalServiceManager
    {
        private static readonly List<Issue> _issues = new List<Issue>();
        private static int _points = 0;

        public static IReadOnlyList<Issue> Issues => _issues.AsReadOnly();
        public static int Points => _points;

        public static void AddIssue(Issue issue, int pointsEarned = 10)
        {
            // This method is deprecated - use DataService.ProcessNewIssue instead
            // Keeping for backward compatibility but not recommended
            issue.UserPoints = CalculatePoints(issue);
            _issues.Add(issue);
            _points += issue.UserPoints;
        }

        public static Issue? GetIssueById(Guid id)
        {
            return _issues.FirstOrDefault(i => i.Id == id);
        }

        public static IEnumerable<Issue> GetIssuesByCategory(IssueCategory category)
        {
            return _issues.Where(i => i.Category == category).OrderByDescending(i => i.SubmittedAt);
        }

        public static IEnumerable<Issue> GetIssuesByStatus(IssueStatus status)
        {
            return _issues.Where(i => i.Status == status).OrderByDescending(i => i.SubmittedAt);
        }

        public static void UpdateIssueStatus(Guid id, IssueStatus status)
        {
            var issue = _issues.FirstOrDefault(i => i.Id == id);
            if (issue != null)
            {
                issue.Status = status;
                if (status == IssueStatus.Resolved || status == IssueStatus.Closed)
                {
                    issue.ResolvedDate = DateTime.UtcNow;
                }
            }
        }

        public static MunicipalStats GetStats()
        {
            return new MunicipalStats
            {
                TotalIssues = _issues.Count,
                ResolvedIssues = _issues.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed),
                TotalPoints = _points,
                OpenIssues = _issues.Count(i => i.Status == IssueStatus.Open),
                InProgressIssues = _issues.Count(i => i.Status == IssueStatus.InProgress)
            };
        }

        private static int CalculatePoints(Issue issue)
        {
            int basePoints = 10;
            
            // Bonus points for priority
            switch (issue.Priority)
            {
                case IssuePriority.Critical:
                    basePoints += 20;
                    break;
                case IssuePriority.High:
                    basePoints += 15;
                    break;
                case IssuePriority.Medium:
                    basePoints += 10;
                    break;
                case IssuePriority.Low:
                    basePoints += 5;
                    break;
            }

            // Bonus points for media attachments
            if (issue.AttachmentPaths.Any())
            {
                basePoints += 5;
            }

            return basePoints;
        }
    }

    public class MunicipalStats
    {
        public int TotalIssues { get; set; }
        public int ResolvedIssues { get; set; }
        public int TotalPoints { get; set; }
        public int OpenIssues { get; set; }
        public int InProgressIssues { get; set; }
        public double ResolutionRate => TotalIssues > 0 ? (double)ResolvedIssues / TotalIssues * 100 : 0;
    }
}
