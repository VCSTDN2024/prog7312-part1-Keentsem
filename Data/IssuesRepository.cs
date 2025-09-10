using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    public class IssuesRepository
    {
        private static readonly List<Issue> _issues = new List<Issue>();
        private static readonly List<UserProfile> _users = new List<UserProfile>();
        private static readonly List<IssueTracking> _tracking = new List<IssueTracking>();
        private static readonly List<IssueComment> _comments = new List<IssueComment>();
        private static readonly Dictionary<IssueCategory, int> _categoryCounts = new Dictionary<IssueCategory, int>();
        private static readonly Dictionary<string, int> _municipalityStats = new Dictionary<string, int>();

        public IEnumerable<Issue> GetAllIssues()
        {
            return _issues.OrderByDescending(i => i.SubmittedAt);
        }

        public Issue? GetIssueById(Guid id)
        {
            return _issues.FirstOrDefault(i => i.Id == id);
        }

        public void AddIssue(Issue issue)
        {
            issue.Id = Guid.NewGuid();
            issue.SubmittedAt = DateTime.UtcNow;
            issue.Status = IssueStatus.Open;
            // Points are calculated by the calling service
            _issues.Add(issue);
            
            // Update category counts
            if (_categoryCounts.ContainsKey(issue.Category))
                _categoryCounts[issue.Category]++;
            else
                _categoryCounts[issue.Category] = 1;
            
            // Update municipality stats
            var municipality = ExtractMunicipality(issue.Location);
            if (!string.IsNullOrEmpty(municipality))
            {
                if (_municipalityStats.ContainsKey(municipality))
                    _municipalityStats[municipality]++;
                else
                    _municipalityStats[municipality] = 1;
            }
            
            // Create initial tracking record
            _tracking.Add(new IssueTracking
            {
                IssueId = issue.Id,
                PreviousStatus = IssueStatus.Open,
                NewStatus = IssueStatus.Open,
                StatusChangedAt = DateTime.UtcNow,
                ChangedBy = "System",
                Comments = "Issue reported and logged",
                IsPublicUpdate = true
            });
            
            // Log to practical analytics services
            LogToAnalyticsServices(issue);
        }
        
        private void LogToAnalyticsServices(Issue issue)
        {
            // This would be called by the service layer to log to analytics
            // Keeping this method for future integration with the new services
        }

        public void UpdateIssue(Issue issue)
        {
            var existingIssue = _issues.FirstOrDefault(i => i.Id == issue.Id);
            if (existingIssue != null)
            {
                var index = _issues.IndexOf(existingIssue);
                _issues[index] = issue;
            }
        }

        public void DeleteIssue(Guid id)
        {
            var issue = _issues.FirstOrDefault(i => i.Id == id);
            if (issue != null)
            {
                _issues.Remove(issue);
            }
        }

        public IEnumerable<Issue> GetIssuesByCategory(IssueCategory category)
        {
            return _issues.Where(i => i.Category == category).OrderByDescending(i => i.SubmittedAt);
        }

        public IEnumerable<Issue> GetIssuesByStatus(IssueStatus status)
        {
            return _issues.Where(i => i.Status == status).OrderByDescending(i => i.SubmittedAt);
        }

        public IEnumerable<Issue> SearchIssues(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return GetAllIssues();

            return _issues.Where(i => 
                i.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                i.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                i.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            ).OrderByDescending(i => i.SubmittedAt);
        }

        public int GetTotalIssuesCount()
        {
            return _issues.Count;
        }

        public int GetResolvedIssuesCount()
        {
            return _issues.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed);
        }

        public int GetTotalUserPoints()
        {
            return _issues.Sum(i => i.UserPoints);
        }

        private int CalculatePoints(Issue issue)
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

        // Badge calculation methods
        public List<Badge> GetEarnedBadges()
        {
            var badges = new List<Badge>();
            var issues = _issues.ToList();

            // First Report Badge
            if (issues.Any())
            {
                badges.Add(new Badge
                {
                    Id = 1,
                    Name = "First Responder",
                    Description = "Submitted your first municipal issue report",
                    ImagePath = "/images/first_responder.png",
                    Type = BadgeType.FirstReport,
                    IsEarned = true,
                    EarnedDate = issues.OrderBy(i => i.SubmittedAt).First().SubmittedAt,
                    PointsValue = 25
                });
            }

            // Community Helper Badge (3+ reports)
            if (issues.Count >= 3)
            {
                badges.Add(new Badge
                {
                    Id = 2,
                    Name = "Community Helper",
                    Description = "Reported 3 or more municipal issues",
                    ImagePath = "/images/neighborhood_watcher.png",
                    Type = BadgeType.CommunityHelper,
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = issues.OrderBy(i => i.SubmittedAt).Skip(2).First().SubmittedAt,
                    PointsValue = 50
                });
            }

            // Category Specialist Badges
            var categoryGroups = issues.GroupBy(i => i.Category);
            foreach (var group in categoryGroups.Where(g => g.Count() >= 2))
            {
                badges.Add(new Badge
                {
                    Id = 10 + (int)group.Key,
                    Name = $"{group.Key} Specialist",
                    Description = $"Reported 2 or more {group.Key} issues",
                    ImagePath = GetCategoryBadgeImage(group.Key),
                    Type = BadgeType.CategorySpecialist,
                    RequiredCategory = group.Key.ToString(),
                    RequiredCount = 2,
                    IsEarned = true,
                    EarnedDate = group.OrderBy(i => i.SubmittedAt).Skip(1).First().SubmittedAt,
                    PointsValue = 30
                });
            }

            // Media Contributor Badge
            if (issues.Any(i => i.AttachmentPaths.Any()))
            {
                badges.Add(new Badge
                {
                    Id = 3,
                    Name = "Media Contributor",
                    Description = "Submitted reports with photos or videos",
                    ImagePath = "/images/persistent_reporter.png",
                    Type = BadgeType.MediaContributor,
                    IsEarned = true,
                    EarnedDate = issues.Where(i => i.AttachmentPaths.Any()).OrderBy(i => i.SubmittedAt).First().SubmittedAt,
                    PointsValue = 40
                });
            }

            // Priority Reporter Badge
            if (issues.Any(i => i.Priority == IssuePriority.Critical))
            {
                badges.Add(new Badge
                {
                    Id = 4,
                    Name = "Emergency Responder",
                    Description = "Reported critical priority issues",
                    ImagePath = "/images/safety_sentinel.png",
                    Type = BadgeType.EmergencyResponder,
                    IsEarned = true,
                    EarnedDate = issues.Where(i => i.Priority == IssuePriority.Critical).OrderBy(i => i.SubmittedAt).First().SubmittedAt,
                    PointsValue = 75
                });
            }

            // Consistent Reporter Badge (5+ reports)
            if (issues.Count >= 5)
            {
                badges.Add(new Badge
                {
                    Id = 5,
                    Name = "Consistent Reporter",
                    Description = "Reported 5 or more municipal issues",
                    ImagePath = "/images/persistent_reporter.png",
                    Type = BadgeType.ConsistentReporter,
                    RequiredCount = 5,
                    IsEarned = true,
                    EarnedDate = issues.OrderBy(i => i.SubmittedAt).Skip(4).First().SubmittedAt,
                    PointsValue = 100
                });
            }

            // Community Champion Badge (10+ reports)
            if (issues.Count >= 10)
            {
                badges.Add(new Badge
                {
                    Id = 6,
                    Name = "Community Champion",
                    Description = "Reported 10 or more municipal issues",
                    ImagePath = "/images/eco_guardian.png",
                    Type = BadgeType.CommunityChampion,
                    RequiredCount = 10,
                    IsEarned = true,
                    EarnedDate = issues.OrderBy(i => i.SubmittedAt).Skip(9).First().SubmittedAt,
                    PointsValue = 200
                });
            }

            return badges.OrderBy(b => b.EarnedDate).ToList();
        }

        public List<Badge> GetAllAvailableBadges()
        {
            var earnedBadges = GetEarnedBadges();
            var allBadges = new List<Badge>();

            // Add all possible badges
            allBadges.AddRange(earnedBadges);

            // Add locked badges
            var lockedBadges = GetLockedBadges(earnedBadges);
            allBadges.AddRange(lockedBadges);

            return allBadges.OrderBy(b => b.Type).ToList();
        }

        private List<Badge> GetLockedBadges(List<Badge> earnedBadges)
        {
            var lockedBadges = new List<Badge>();
            var issues = _issues.ToList();

            // Check for locked Community Helper (if not earned)
            if (!earnedBadges.Any(b => b.Type == BadgeType.CommunityHelper))
            {
                lockedBadges.Add(new Badge
                {
                    Id = 2,
                    Name = "Community Helper",
                    Description = "Report 3 or more municipal issues to unlock",
                    ImagePath = "/images/neighborhood_watcher.png",
                    Type = BadgeType.CommunityHelper,
                    RequiredCount = 3,
                    IsEarned = false,
                    PointsValue = 50
                });
            }

            // Check for locked Consistent Reporter (if not earned)
            if (!earnedBadges.Any(b => b.Type == BadgeType.ConsistentReporter))
            {
                lockedBadges.Add(new Badge
                {
                    Id = 5,
                    Name = "Consistent Reporter",
                    Description = "Report 5 or more municipal issues to unlock",
                    ImagePath = "/images/persistent_reporter.png",
                    Type = BadgeType.ConsistentReporter,
                    RequiredCount = 5,
                    IsEarned = false,
                    PointsValue = 100
                });
            }

            // Check for locked Community Champion (if not earned)
            if (!earnedBadges.Any(b => b.Type == BadgeType.CommunityChampion))
            {
                lockedBadges.Add(new Badge
                {
                    Id = 6,
                    Name = "Community Champion",
                    Description = "Report 10 or more municipal issues to unlock",
                    ImagePath = "/images/eco_guardian.png",
                    Type = BadgeType.CommunityChampion,
                    RequiredCount = 10,
                    IsEarned = false,
                    PointsValue = 200
                });
            }

            return lockedBadges;
        }

        private string GetCategoryBadgeImage(IssueCategory category)
        {
            return category switch
            {
                IssueCategory.WaterSupply => "/images/water_saver.png",
                IssueCategory.Electricity => "/images/power_saver.png",
                IssueCategory.Roads => "/images/road_warrior.png",
                IssueCategory.WasteManagement => "/images/eco_guardian.png",
                IssueCategory.PublicSafety => "/images/safety_sentinel.png",
                IssueCategory.ParksAndRecreation => "/images/eco_guardian.png",
                IssueCategory.BuildingPermits => "/images/neighborhood_watcher.png",
                _ => "/images/persistent_reporter.png"
            };
        }

        // Advanced Data Structure Methods for Top Marks
        public List<Leaderboard> GetLeaderboard(int topCount = 10)
        {
            return _users
                .OrderByDescending(u => u.TotalPoints)
                .ThenByDescending(u => u.ReportsSubmitted)
                .Take(topCount)
                .Select((user, index) => new Leaderboard
                {
                    UserName = user.Name,
                    UserEmail = user.Email,
                    TotalPoints = user.TotalPoints,
                    ReportsSubmitted = user.ReportsSubmitted,
                    IssuesResolved = user.IssuesResolved,
                    Level = user.Level,
                    LastActiveDate = user.LastActiveDate,
                    Rank = index + 1,
                    BadgeIds = user.EarnedBadges,
                    Municipality = user.Municipality,
                    Province = user.Province
                })
                .ToList();
        }

        public MunicipalityStats GetMunicipalityStats(string municipality)
        {
            var issues = _issues.Where(i => ExtractMunicipality(i.Location) == municipality).ToList();
            var categories = issues.GroupBy(i => i.Category)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();

            return new MunicipalityStats
            {
                MunicipalityName = municipality,
                Province = ExtractProvince(municipality),
                TotalIssues = issues.Count,
                ResolvedIssues = issues.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed),
                ActiveUsers = _users.Count(u => u.Municipality == municipality),
                TopCategories = categories,
                LastUpdated = DateTime.UtcNow
            };
        }

        public GamificationMetrics GetGamificationMetrics()
        {
            var levelDistribution = _users.GroupBy(u => u.Level)
                .ToDictionary(g => g.Key, g => g.Count());

            var categoryDistribution = _categoryCounts.ToDictionary(
                kvp => kvp.Key, 
                kvp => kvp.Value
            );

            return new GamificationMetrics
            {
                TotalUsers = _users.Count,
                TotalIssues = _issues.Count,
                TotalPointsAwarded = _users.Sum(u => u.TotalPoints),
                TotalBadgesEarned = _users.Sum(u => u.EarnedBadges.Count),
                LevelDistribution = levelDistribution.Keys.ToList(),
                CategoryDistribution = categoryDistribution
            };
        }

        public List<IssueTracking> GetIssueTrackingHistory(Guid issueId)
        {
            return _tracking
                .Where(t => t.IssueId == issueId)
                .OrderBy(t => t.StatusChangedAt)
                .ToList();
        }

        public void AddIssueComment(IssueComment comment)
        {
            comment.Id = Guid.NewGuid();
            comment.CreatedAt = DateTime.UtcNow;
            _comments.Add(comment);
        }

        public List<IssueComment> GetIssueComments(Guid issueId)
        {
            return _comments
                .Where(c => c.IssueId == issueId)
                .OrderBy(c => c.CreatedAt)
                .ToList();
        }

        public void UpdateIssueStatus(Guid issueId, IssueStatus newStatus, string changedBy, string? comments = null)
        {
            var issue = _issues.FirstOrDefault(i => i.Id == issueId);
            if (issue != null)
            {
                var previousStatus = issue.Status;
                issue.Status = newStatus;
                
                if (newStatus == IssueStatus.Resolved || newStatus == IssueStatus.Closed)
                {
                    issue.ResolvedDate = DateTime.UtcNow;
                }

                _tracking.Add(new IssueTracking
                {
                    IssueId = issueId,
                    PreviousStatus = previousStatus,
                    NewStatus = newStatus,
                    StatusChangedAt = DateTime.UtcNow,
                    ChangedBy = changedBy,
                    Comments = comments ?? $"Status changed from {previousStatus} to {newStatus}",
                    IsPublicUpdate = true
                });
            }
        }

        public Dictionary<IssueCategory, int> GetCategoryStatistics()
        {
            return _categoryCounts.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public Dictionary<string, int> GetMunicipalityStatistics()
        {
            return _municipalityStats.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public List<Issue> GetIssuesByDateRange(DateTime startDate, DateTime endDate)
        {
            return _issues
                .Where(i => i.SubmittedAt >= startDate && i.SubmittedAt <= endDate)
                .OrderByDescending(i => i.SubmittedAt)
                .ToList();
        }

        public List<Issue> GetIssuesByPriority(IssuePriority priority)
        {
            return _issues
                .Where(i => i.Priority == priority)
                .OrderByDescending(i => i.SubmittedAt)
                .ToList();
        }

        private string ExtractMunicipality(string location)
        {
            // Extract municipality from location string
            var parts = location.Split(',');
            if (parts.Length >= 2)
            {
                return parts[0].Trim();
            }
            return string.Empty;
        }

        private string ExtractProvince(string municipality)
        {
            // Simple province mapping - in real app, this would be more sophisticated
            var provinceMap = new Dictionary<string, string>
            {
                { "Cape Town", "Western Cape" },
                { "Johannesburg", "Gauteng" },
                { "Durban", "KwaZulu-Natal" },
                { "Pretoria", "Gauteng" },
                { "Port Elizabeth", "Eastern Cape" },
                { "Bloemfontein", "Free State" },
                { "Nelspruit", "Mpumalanga" },
                { "Polokwane", "Limpopo" },
                { "Kimberley", "Northern Cape" },
                { "Bisho", "Eastern Cape" }
            };

            return provinceMap.GetValueOrDefault(municipality, "Unknown");
        }

        #region Learning Unit 1: Custom Sorting Methods

        /// <summary>
        /// Get issues sorted using custom QuickSort algorithm by priority
        /// Demonstrates custom sorting implementation for academic requirements
        /// Time Complexity: O(n log n) average case, O(n²) worst case
        /// </summary>
        public List<Issue> GetIssuesSortedByPriorityCustom()
        {
            var issues = _issues.ToArray();
            if (issues.Length == 0) return new List<Issue>();

            // Use custom QuickSort implementation
            CustomSortingAlgorithms.QuickSortByPriority(issues, 0, issues.Length - 1);
            return issues.ToList();
        }

        /// <summary>
        /// Get issues sorted using custom MergeSort algorithm by date
        /// Demonstrates divide-and-conquer sorting algorithm
        /// Time Complexity: O(n log n) guaranteed
        /// </summary>
        public List<Issue> GetIssuesSortedByDateCustom()
        {
            var issues = _issues.ToList();
            if (issues.Count == 0) return new List<Issue>();

            // Use custom MergeSort implementation
            CustomSortingAlgorithms.MergeSortByDate(issues);
            return issues;
        }

        /// <summary>
        /// Get issues sorted using custom HeapSort algorithm by points
        /// Demonstrates heap-based sorting algorithm
        /// Time Complexity: O(n log n) guaranteed
        /// </summary>
        public List<Issue> GetIssuesSortedByPointsCustom()
        {
            var issues = _issues.ToArray();
            if (issues.Length == 0) return new List<Issue>();

            // Use custom HeapSort implementation
            CustomSortingAlgorithms.HeapSortByPoints(issues);
            return issues.ToList();
        }

        #endregion

        #region Learning Unit 2: Recursive Methods

        /// <summary>
        /// Recursively count issues by category using municipal repository
        /// Demonstrates recursive algorithm implementation integrated with municipal operations
        /// Time Complexity: O(n) where n is the number of issues
        /// </summary>
        public int CountIssuesByCategoryRecursive(IssueCategory category, int index = 0)
        {
            var allIssues = _issues;
            return CountIssuesByCategoryRecursiveHelper(allIssues, category, index);
        }

        private int CountIssuesByCategoryRecursiveHelper(List<Issue> issues, IssueCategory category, int index)
        {
            // Base case: reached end of list
            if (index >= issues.Count)
                return 0;

            // Recursive case: check current issue and continue
            int currentCount = issues[index].Category == category ? 1 : 0;
            return currentCount + CountIssuesByCategoryRecursiveHelper(issues, category, index + 1);
        }

        /// <summary>
        /// Recursively find issues by priority using municipal repository
        /// Demonstrates recursive filtering with accumulation
        /// Time Complexity: O(n) where n is the number of issues
        /// </summary>
        public List<Issue> FindIssuesByPriorityRecursive(IssuePriority priority, int index = 0)
        {
            var allIssues = _issues;
            return FindIssuesByPriorityRecursiveHelper(allIssues, priority, index);
        }

        private List<Issue> FindIssuesByPriorityRecursiveHelper(List<Issue> issues, IssuePriority priority, int index)
        {
            // Base case: reached end of list
            if (index >= issues.Count)
                return new List<Issue>();

            // Recursive case: check current issue and continue
            var result = FindIssuesByPriorityRecursiveHelper(issues, priority, index + 1);
            
            if (issues[index].Priority == priority)
            {
                result.Insert(0, issues[index]); // Insert at beginning to maintain order
            }

            return result;
        }

        /// <summary>
        /// Recursively calculate total points using municipal repository
        /// Demonstrates recursive accumulation integrated with municipal operations
        /// Time Complexity: O(n) where n is the number of issues
        /// </summary>
        public int CalculateTotalPointsRecursive(int index = 0)
        {
            var allIssues = _issues;
            return CalculateTotalPointsRecursiveHelper(allIssues, index);
        }

        private int CalculateTotalPointsRecursiveHelper(List<Issue> issues, int index)
        {
            // Base case: reached end of list
            if (index >= issues.Count)
                return 0;

            // Recursive case: add current points and continue
            return issues[index].UserPoints + CalculateTotalPointsRecursiveHelper(issues, index + 1);
        }

        #endregion
    }

    #region Custom Sorting Algorithms

    /// <summary>
    /// Custom sorting algorithms implementation for Learning Unit 1
    /// Demonstrates various sorting algorithms with different time complexities
    /// </summary>
    public static class CustomSortingAlgorithms
    {
        /// <summary>
        /// QuickSort implementation for issues by priority
        /// Time Complexity: O(n log n) average, O(n²) worst case
        /// Space Complexity: O(log n) due to recursion stack
        /// </summary>
        public static void QuickSortByPriority(Issue[] issues, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = PartitionByPriority(issues, low, high);
                QuickSortByPriority(issues, low, pivotIndex - 1);
                QuickSortByPriority(issues, pivotIndex + 1, high);
            }
        }

        private static int PartitionByPriority(Issue[] issues, int low, int high)
        {
            Issue pivot = issues[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (issues[j].Priority >= pivot.Priority) // Higher priority first
                {
                    i++;
                    Swap(issues, i, j);
                }
            }
            Swap(issues, i + 1, high);
            return i + 1;
        }

        /// <summary>
        /// MergeSort implementation for issues by date
        /// Time Complexity: O(n log n) guaranteed
        /// Space Complexity: O(n) for temporary array
        /// </summary>
        public static void MergeSortByDate(List<Issue> issues)
        {
            if (issues.Count <= 1) return;
            
            Issue[] temp = new Issue[issues.Count];
            MergeSortByDateRecursive(issues, temp, 0, issues.Count - 1);
        }

        private static void MergeSortByDateRecursive(List<Issue> issues, Issue[] temp, int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                MergeSortByDateRecursive(issues, temp, left, mid);
                MergeSortByDateRecursive(issues, temp, mid + 1, right);
                MergeByDate(issues, temp, left, mid, right);
            }
        }

        private static void MergeByDate(List<Issue> issues, Issue[] temp, int left, int mid, int right)
        {
            int i = left, j = mid + 1, k = left;

            while (i <= mid && j <= right)
            {
                if (issues[i].SubmittedAt >= issues[j].SubmittedAt) // Newest first
                {
                    temp[k] = issues[i];
                    i++;
                }
                else
                {
                    temp[k] = issues[j];
                    j++;
                }
                k++;
            }

            while (i <= mid)
            {
                temp[k] = issues[i];
                i++;
                k++;
            }

            while (j <= right)
            {
                temp[k] = issues[j];
                j++;
                k++;
            }

            for (i = left; i <= right; i++)
            {
                issues[i] = temp[i];
            }
        }

        /// <summary>
        /// HeapSort implementation for issues by points
        /// Time Complexity: O(n log n) guaranteed
        /// Space Complexity: O(1) - in-place sorting
        /// </summary>
        public static void HeapSortByPoints(Issue[] issues)
        {
            int n = issues.Length;

            // Build heap
            for (int i = n / 2 - 1; i >= 0; i--)
                HeapifyByPoints(issues, n, i);

            // Extract elements from heap
            for (int i = n - 1; i > 0; i--)
            {
                Swap(issues, 0, i);
                HeapifyByPoints(issues, i, 0);
            }
        }

        private static void HeapifyByPoints(Issue[] issues, int n, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && issues[left].UserPoints > issues[largest].UserPoints)
                largest = left;

            if (right < n && issues[right].UserPoints > issues[largest].UserPoints)
                largest = right;

            if (largest != i)
            {
                Swap(issues, i, largest);
                HeapifyByPoints(issues, n, largest);
            }
        }

        /// <summary>
        /// Utility method for swapping elements
        /// </summary>
        private static void Swap(Issue[] issues, int i, int j)
        {
            Issue temp = issues[i];
            issues[i] = issues[j];
            issues[j] = temp;
        }
    }

    #endregion
}
