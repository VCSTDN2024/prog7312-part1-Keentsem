using MunicipalServicesApp.Models;
using MunicipalServicesApp.Data;
using System.Collections.Concurrent;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Enhanced Issue Management Service with advanced data structures and event handling
    /// Implements proper data organization for municipal issue management
    /// </summary>
    public class IssueManagementService
    {
        private readonly IssuesRepository _issuesRepository;
        private readonly NotificationService _notificationService;
        private readonly BadgeService _badgeService;
        private readonly DataService _dataService;
        private readonly UserRepository _userRepository;
        
        // ========================================
        // ADVANCED DATA STRUCTURES - Collections & Arrays
        // ========================================
        // These data structures provide efficient data organization and retrieval
        
        /// <summary>
        /// ConcurrentDictionary with ConcurrentQueue for category-based issue queuing
        /// Purpose: Thread-safe FIFO processing of issues by category
        /// Usage: Prioritized issue processing, category-specific workflows
        /// </summary>
        private readonly ConcurrentDictionary<IssueCategory, ConcurrentQueue<Issue>> _categoryQueues;
        
        /// <summary>
        /// ConcurrentDictionary with ConcurrentBag for priority-based issue collection
        /// Purpose: Thread-safe unordered collection of issues by priority level
        /// Usage: Emergency response, priority-based notifications, analytics
        /// </summary>
        private readonly ConcurrentDictionary<IssuePriority, ConcurrentBag<Issue>> _priorityBags;
        
        /// <summary>
        /// ConcurrentDictionary with List for location-based issue indexing
        /// Purpose: Fast location-based search and geographic analysis
        /// Usage: Map displays, location-based filtering, regional analytics
        /// </summary>
        private readonly ConcurrentDictionary<string, List<Issue>> _locationIndex;
        
        /// <summary>
        /// ConcurrentDictionary with HashSet for status-based issue tracking
        /// Purpose: O(1) lookup for issues by status, duplicate prevention
        /// Usage: Status filtering, workflow management, progress tracking
        /// </summary>
        private readonly ConcurrentDictionary<IssueStatus, HashSet<Guid>> _statusIndex;
        
        // ========================================
        // MULTI-DIMENSIONAL ARRAYS - Advanced Analytics
        // ========================================
        // These arrays provide efficient storage and analysis of complex data relationships
        
        /// <summary>
        /// 2D Array: Category vs Priority Matrix
        /// Dimensions: [CategoryIndex, PriorityIndex]
        /// Purpose: Track issue distribution across categories and priorities
        /// Usage: Analytics dashboard, trend analysis, resource allocation
        /// </summary>
        private readonly int[,] _categoryPriorityMatrix;
        
        /// <summary>
        /// 3D Array: Location vs Category vs Priority Matrix
        /// Dimensions: [LocationZone, CategoryIndex, PriorityIndex]
        /// Purpose: Geographic analysis of issue patterns and priorities
        /// Usage: Regional analytics, hotspot identification, resource planning
        /// </summary>
        private readonly int[,,] _locationCategoryPriorityMatrix;
        
        // ========================================
        // EVENT HANDLERS - Observer Pattern Implementation
        // ========================================
        // These events implement the Observer pattern for loose coupling
        // between components and real-time notifications
        
        /// <summary>
        /// Event triggered when a new issue is submitted
        /// Subscribers: NotificationService, AnalyticsService, UI components
        /// Usage: Real-time notifications, dashboard updates, analytics tracking
        /// </summary>
        public event EventHandler<IssueEventArgs>? IssueSubmitted;
        
        /// <summary>
        /// Event triggered when an issue status changes (e.g., InProgress -> Resolved)
        /// Subscribers: NotificationService, UserSessionService, UI components
        /// Usage: Status update notifications, progress tracking, user feedback
        /// </summary>
        public event EventHandler<IssueEventArgs>? IssueStatusChanged;
        
        /// <summary>
        /// Event triggered when a user earns a new badge
        /// Subscribers: NotificationService, UserSessionService, UI components
        /// Usage: Badge notifications, achievement celebrations, leaderboard updates
        /// </summary>
        public event EventHandler<BadgeEarnedEventArgs>? BadgeEarned;
        
        /// <summary>
        /// Event triggered when an issue is marked as resolved
        /// Subscribers: NotificationService, AnalyticsService, UI components
        /// Usage: Resolution notifications, completion tracking, user satisfaction
        /// </summary>
        public event EventHandler<IssueEventArgs>? IssueResolved;

        public IssueManagementService(
            IssuesRepository issuesRepository, 
            NotificationService notificationService,
            BadgeService badgeService,
            DataService dataService,
            UserRepository userRepository)
        {
            _issuesRepository = issuesRepository;
            _notificationService = notificationService;
            _badgeService = badgeService;
            _dataService = dataService;
            _userRepository = userRepository;
            
            // Initialize data structures
            _categoryQueues = new ConcurrentDictionary<IssueCategory, ConcurrentQueue<Issue>>();
            _priorityBags = new ConcurrentDictionary<IssuePriority, ConcurrentBag<Issue>>();
            _locationIndex = new ConcurrentDictionary<string, List<Issue>>();
            _statusIndex = new ConcurrentDictionary<IssueStatus, HashSet<Guid>>();
            
            // Initialize multi-dimensional arrays
            _categoryPriorityMatrix = new int[Enum.GetValues<IssueCategory>().Length, Enum.GetValues<IssuePriority>().Length];
            _locationCategoryPriorityMatrix = new int[10, Enum.GetValues<IssueCategory>().Length, Enum.GetValues<IssuePriority>().Length]; // 10 location zones
            
            InitializeDataStructures();
        }

        /// <summary>
        /// Initialize all data structures with existing issues
        /// </summary>
        private void InitializeDataStructures()
        {
            var issues = _issuesRepository.GetAllIssues().ToList();
            
            foreach (var issue in issues)
            {
                AddToDataStructures(issue);
            }
        }

        /// <summary>
        /// Submit a new issue with enhanced processing and event handling
        /// </summary>
        public async Task<IssueSubmissionResult> SubmitIssueAsync(Issue issue)
        {
            try
            {
                // Calculate points
                issue.UserPoints = CalculateIssuePoints(issue);
                
                // Add to repository
                _issuesRepository.AddIssue(issue);
                
                // Update user session with points
                var userSession = _dataService.GetOrCreateUserSession(issue.UserEmail ?? "demo@municipal.gov.za", "Demo User", "Cape Town");
                userSession.TotalPoints += issue.UserPoints;
                userSession.ReportsSubmitted++;
                userSession.ReportedIssues.Add(issue.Id);
                userSession.LastActiveAt = DateTime.UtcNow;
                userSession.Level = CalculateUserLevel(userSession.TotalPoints);
                _dataService.UpdateUserSession(userSession);
                
                // Also update the UserRepository for consistency
                var user = _userRepository.GetUserByEmail(issue.UserEmail ?? "demo@municipal.gov.za");
                if (user != null)
                {
                    user.TotalPoints = userSession.TotalPoints;
                    user.Level = userSession.Level;
                    user.LastActiveAt = DateTime.UtcNow;
                    _userRepository.UpdateUser(user);
                    
                    // Update multi-dimensional array
                    UserRepository.UpdateUserPointsInArray(user.Id, user.TotalPoints);
                    UserRepository.UpdateUserReportsCount(user.Id, userSession.ReportsSubmitted);
                }
                
                // Add to data structures
                AddToDataStructures(issue);
                
                // Update matrices
                UpdateMatrices(issue);
                
                // Check for badge earning
                var badgeResult = await CheckForBadgeEarningAsync(issue);
                
                // Create notifications
                await CreateIssueNotificationsAsync(issue, badgeResult);
                
                // Raise events
                OnIssueSubmitted(new IssueEventArgs(issue));
                
                return new IssueSubmissionResult
                {
                    Success = true,
                    Issue = issue,
                    PointsAwarded = issue.UserPoints,
                    BadgesEarned = badgeResult.BadgesEarned,
                    NotificationsCreated = badgeResult.NotificationsCreated
                };
            }
            catch (Exception ex)
            {
                return new IssueSubmissionResult
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Update issue status with proper event handling
        /// </summary>
        public async Task<bool> UpdateIssueStatusAsync(Guid issueId, IssueStatus newStatus, string changedBy, string? comments = null)
        {
            var issue = _issuesRepository.GetIssueById(issueId);
            if (issue == null) return false;

            var oldStatus = issue.Status;
            issue.Status = newStatus;
            issue.LastModified = DateTime.UtcNow;
            
            if (newStatus == IssueStatus.Resolved || newStatus == IssueStatus.Closed)
            {
                issue.ResolvedDate = DateTime.UtcNow;
            }

            _issuesRepository.UpdateIssue(issue);
            
            // Update data structures
            UpdateStatusInDataStructures(issueId, oldStatus, newStatus);
            
            // Create notification
            await _notificationService.CreateNotificationAsync(new Notification
            {
                UserEmail = issue.UserEmail ?? "system@municipal.gov.za",
                Type = NotificationType.IssueStatusChanged,
                Title = $"Issue Status Updated",
                Message = $"Your issue '{issue.Title}' status has been changed from {oldStatus} to {newStatus}",
                RelatedIssueId = issueId,
                PointsAwarded = newStatus == IssueStatus.Resolved ? 10 : 0
            });
            
            // Raise event
            OnIssueStatusChanged(new IssueEventArgs(issue));
            
            if (newStatus == IssueStatus.Resolved)
            {
                OnIssueResolved(new IssueEventArgs(issue));
            }

            return true;
        }

        /// <summary>
        /// Get issues organized by category using advanced data structures
        /// </summary>
        public Dictionary<IssueCategory, List<Issue>> GetIssuesByCategory()
        {
            var result = new Dictionary<IssueCategory, List<Issue>>();
            
            foreach (var category in Enum.GetValues<IssueCategory>())
            {
                if (_categoryQueues.TryGetValue(category, out var queue))
                {
                    result[category] = queue.ToList();
                }
                else
                {
                    result[category] = new List<Issue>();
                }
            }
            
            return result;
        }

        /// <summary>
        /// Get issues by priority using concurrent collections
        /// </summary>
        public Dictionary<IssuePriority, List<Issue>> GetIssuesByPriority()
        {
            var result = new Dictionary<IssuePriority, List<Issue>>();
            
            foreach (var priority in Enum.GetValues<IssuePriority>())
            {
                if (_priorityBags.TryGetValue(priority, out var bag))
                {
                    result[priority] = bag.ToList();
                }
                else
                {
                    result[priority] = new List<Issue>();
                }
            }
            
            return result;
        }

        /// <summary>
        /// Get issues by location with spatial indexing
        /// </summary>
        public Dictionary<string, List<Issue>> GetIssuesByLocation()
        {
            return _locationIndex.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToList());
        }

        /// <summary>
        /// Get issues by status with efficient lookup
        /// </summary>
        public Dictionary<IssueStatus, List<Issue>> GetIssuesByStatus()
        {
            var result = new Dictionary<IssueStatus, List<Issue>>();
            
            foreach (var status in Enum.GetValues<IssueStatus>())
            {
                if (_statusIndex.TryGetValue(status, out var issueIds))
                {
                    result[status] = issueIds.Select(id => _issuesRepository.GetIssueById(id))
                                           .Where(issue => issue != null)
                                           .Cast<Issue>()
                                           .ToList();
                }
                else
                {
                    result[status] = new List<Issue>();
                }
            }
            
            return result;
        }

        /// <summary>
        /// Get advanced analytics using multi-dimensional arrays
        /// </summary>
        public IssueAnalytics GetIssueAnalytics()
        {
            return new IssueAnalytics
            {
                CategoryPriorityMatrix = _categoryPriorityMatrix,
                LocationCategoryPriorityMatrix = _locationCategoryPriorityMatrix,
                TotalIssues = _issuesRepository.GetTotalIssuesCount(),
                ResolvedIssues = _issuesRepository.GetResolvedIssuesCount(),
                AverageResolutionTime = CalculateAverageResolutionTime(),
                CategoryDistribution = GetCategoryDistribution(),
                PriorityDistribution = GetPriorityDistribution(),
                LocationDistribution = GetLocationDistribution()
            };
        }

        #region Private Methods

        private void AddToDataStructures(Issue issue)
        {
            // Add to category queue
            _categoryQueues.GetOrAdd(issue.Category, _ => new ConcurrentQueue<Issue>()).Enqueue(issue);
            
            // Add to priority bag
            _priorityBags.GetOrAdd(issue.Priority, _ => new ConcurrentBag<Issue>()).Add(issue);
            
            // Add to location index
            var location = ExtractLocationKey(issue.Location);
            _locationIndex.AddOrUpdate(location, 
                new List<Issue> { issue }, 
                (key, existing) => { existing.Add(issue); return existing; });
            
            // Add to status index
            _statusIndex.AddOrUpdate(issue.Status,
                new HashSet<Guid> { issue.Id },
                (key, existing) => { existing.Add(issue.Id); return existing; });
        }

        private void UpdateStatusInDataStructures(Guid issueId, IssueStatus oldStatus, IssueStatus newStatus)
        {
            // Remove from old status
            if (_statusIndex.TryGetValue(oldStatus, out var oldSet))
            {
                oldSet.Remove(issueId);
            }
            
            // Add to new status
            _statusIndex.AddOrUpdate(newStatus,
                new HashSet<Guid> { issueId },
                (key, existing) => { existing.Add(issueId); return existing; });
        }

        private void UpdateMatrices(Issue issue)
        {
            var categoryIndex = (int)issue.Category;
            var priorityIndex = (int)issue.Priority - 1; // Priority enum starts at 1
            var locationIndex = GetLocationZoneIndex(issue.Location);
            
            if (categoryIndex >= 0 && categoryIndex < _categoryPriorityMatrix.GetLength(0) &&
                priorityIndex >= 0 && priorityIndex < _categoryPriorityMatrix.GetLength(1))
            {
                _categoryPriorityMatrix[categoryIndex, priorityIndex]++;
            }
            
            if (locationIndex >= 0 && locationIndex < _locationCategoryPriorityMatrix.GetLength(0) &&
                categoryIndex >= 0 && categoryIndex < _locationCategoryPriorityMatrix.GetLength(1) &&
                priorityIndex >= 0 && priorityIndex < _locationCategoryPriorityMatrix.GetLength(2))
            {
                _locationCategoryPriorityMatrix[locationIndex, categoryIndex, priorityIndex]++;
            }
        }

        private Task<BadgeEarningResult> CheckForBadgeEarningAsync(Issue issue)
        {
            var userIssues = _issuesRepository.GetAllIssues()
                .Where(i => i.UserEmail == issue.UserEmail)
                .ToList();

            var newBadges = new List<Badge>();
            var notifications = new List<Notification>();

            // First Report Badge
            if (userIssues.Count == 1)
            {
                var firstReportBadge = new Badge
                {
                    Id = 1,
                    Name = "First Responder",
                    Description = "Submitted your first municipal issue report",
                    ImagePath = "/images/badges/first_responder.png",
                    Type = BadgeType.FirstReport,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 25
                };
                
                newBadges.Add(firstReportBadge);
                notifications.Add(CreateBadgeNotification(firstReportBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Community Helper Badge (3+ reports)
            if (userIssues.Count == 3)
            {
                var communityHelperBadge = new Badge
                {
                    Id = 2,
                    Name = "Community Helper",
                    Description = "Reported 3 or more municipal issues",
                    ImagePath = "/images/badges/community_helper.png",
                    Type = BadgeType.CommunityHelper,
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 50
                };
                
                newBadges.Add(communityHelperBadge);
                notifications.Add(CreateBadgeNotification(communityHelperBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Media Contributor Badge
            if (userIssues.Any(i => i.AttachmentPaths.Any()) && !newBadges.Any(b => b.Type == BadgeType.MediaContributor))
            {
                var mediaContributorBadge = new Badge
                {
                    Id = 3,
                    Name = "Media Contributor",
                    Description = "Submitted reports with photos or videos",
                    ImagePath = "/images/badges/neighborhood_watcher.png",
                    Type = BadgeType.MediaContributor,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 40
                };
                
                newBadges.Add(mediaContributorBadge);
                notifications.Add(CreateBadgeNotification(mediaContributorBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Emergency Responder Badge
            if (userIssues.Any(i => i.Priority == IssuePriority.Critical) && !newBadges.Any(b => b.Type == BadgeType.EmergencyResponder))
            {
                var emergencyResponderBadge = new Badge
                {
                    Id = 4,
                    Name = "Emergency Responder",
                    Description = "Reported critical priority issues",
                    ImagePath = "/images/badges/safety_sentinel.png",
                    Type = BadgeType.EmergencyResponder,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 75
                };
                
                newBadges.Add(emergencyResponderBadge);
                notifications.Add(CreateBadgeNotification(emergencyResponderBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Consistent Reporter Badge (5+ reports)
            if (userIssues.Count == 5)
            {
                var consistentReporterBadge = new Badge
                {
                    Id = 5,
                    Name = "Consistent Reporter",
                    Description = "Reported 5 or more municipal issues",
                    ImagePath = "/images/badges/persistent_reporter.png",
                    Type = BadgeType.ConsistentReporter,
                    RequiredCount = 5,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 100
                };
                
                newBadges.Add(consistentReporterBadge);
                notifications.Add(CreateBadgeNotification(consistentReporterBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Community Champion Badge (10+ reports)
            if (userIssues.Count == 10)
            {
                var communityChampionBadge = new Badge
                {
                    Id = 6,
                    Name = "Community Champion",
                    Description = "Reported 10 or more municipal issues",
                    ImagePath = "/images/badges/community_champion.png",
                    Type = BadgeType.CommunityChampion,
                    RequiredCount = 10,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 200
                };
                
                newBadges.Add(communityChampionBadge);
                notifications.Add(CreateBadgeNotification(communityChampionBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Water Saver Badge (3+ water supply issues)
            var waterIssues = userIssues.Where(i => i.Category == IssueCategory.WaterSupply).ToList();
            if (waterIssues.Count == 3)
            {
                var waterSaverBadge = new Badge
                {
                    Id = 7,
                    Name = "Water Saver",
                    Description = "Reported 3 or more water supply issues",
                    ImagePath = "/images/badges/water_saver.png",
                    Type = BadgeType.CategorySpecialist,
                    RequiredCategory = "WaterSupply",
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 60
                };
                
                newBadges.Add(waterSaverBadge);
                notifications.Add(CreateBadgeNotification(waterSaverBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Power Saver Badge (3+ electricity issues)
            var electricityIssues = userIssues.Where(i => i.Category == IssueCategory.Electricity).ToList();
            if (electricityIssues.Count == 3)
            {
                var powerSaverBadge = new Badge
                {
                    Id = 8,
                    Name = "Power Saver",
                    Description = "Reported 3 or more electricity issues",
                    ImagePath = "/images/badges/power_saver.png",
                    Type = BadgeType.CategorySpecialist,
                    RequiredCategory = "Electricity",
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 60
                };
                
                newBadges.Add(powerSaverBadge);
                notifications.Add(CreateBadgeNotification(powerSaverBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Road Warrior Badge (3+ roads issues)
            var roadsIssues = userIssues.Where(i => i.Category == IssueCategory.Roads).ToList();
            if (roadsIssues.Count == 3)
            {
                var roadWarriorBadge = new Badge
                {
                    Id = 9,
                    Name = "Road Warrior",
                    Description = "Reported 3 or more roads and infrastructure issues",
                    ImagePath = "/images/badges/road_warrior.png",
                    Type = BadgeType.CategorySpecialist,
                    RequiredCategory = "Roads",
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 60
                };
                
                newBadges.Add(roadWarriorBadge);
                notifications.Add(CreateBadgeNotification(roadWarriorBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Eco Guardian Badge (3+ environmental issues)
            var ecoIssues = userIssues.Where(i => i.Category == IssueCategory.WasteManagement || i.Category == IssueCategory.ParksAndRecreation).ToList();
            if (ecoIssues.Count == 3)
            {
                var ecoGuardianBadge = new Badge
                {
                    Id = 10,
                    Name = "Eco Guardian",
                    Description = "Reported 3 or more environmental issues",
                    ImagePath = "/images/badges/eco_guardian.png",
                    Type = BadgeType.CategorySpecialist,
                    RequiredCategory = "Environmental",
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = DateTime.UtcNow,
                    PointsValue = 70
                };
                
                newBadges.Add(ecoGuardianBadge);
                notifications.Add(CreateBadgeNotification(ecoGuardianBadge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            // Raise events for all new badges
            foreach (var badge in newBadges)
            {
                OnBadgeEarned(new BadgeEarnedEventArgs(badge, issue.UserEmail ?? "demo@municipal.gov.za"));
            }

            return Task.FromResult(new BadgeEarningResult
            {
                BadgesEarned = newBadges,
                NotificationsCreated = notifications
            });
        }

        private Notification CreateBadgeNotification(Badge badge, string userEmail)
        {
            return new Notification
            {
                UserEmail = userEmail,
                Type = NotificationType.BadgeEarned,
                Title = "🎉 Badge Earned!",
                Message = $"Congratulations! You've earned the '{badge.Name}' badge!",
                ImagePath = badge.ImagePath,
                PointsAwarded = badge.PointsValue,
                RelatedBadgeId = badge.Id
            };
        }

        private Task CreateIssueNotificationsAsync(Issue issue, BadgeEarningResult badgeResult)
        {
            // Create issue submission notification
            var issueNotification = new Notification
            {
                UserEmail = issue.UserEmail ?? "demo@municipal.gov.za",
                Type = NotificationType.IssueSubmitted,
                Title = "Issue Submitted Successfully",
                Message = $"Your issue '{issue.Title}' has been submitted and is being reviewed. You earned {issue.UserPoints} points!",
                RelatedIssueId = issue.Id,
                PointsAwarded = issue.UserPoints
            };
            _notificationService.CreateNotificationAsync(issueNotification);

            // Create badge notifications
            foreach (var notification in badgeResult.NotificationsCreated)
            {
                _notificationService.CreateNotificationAsync(notification);
            }

            return Task.CompletedTask;
        }

        private int CalculateIssuePoints(Issue issue)
        {
            int basePoints = 10;
            
            switch (issue.Priority)
            {
                case IssuePriority.Critical: basePoints += 20; break;
                case IssuePriority.High: basePoints += 15; break;
                case IssuePriority.Medium: basePoints += 10; break;
                case IssuePriority.Low: basePoints += 5; break;
            }

            if (issue.AttachmentPaths.Any())
            {
                basePoints += 5;
            }

            return basePoints;
        }

        private string ExtractLocationKey(string location)
        {
            var parts = location.Split(',');
            return parts.Length > 0 ? parts[0].Trim() : "Unknown";
        }

        private int GetLocationZoneIndex(string location)
        {
            // Simple zone mapping - in production this would be more sophisticated
            var locationKey = ExtractLocationKey(location).ToLower();
            if (locationKey.Contains("city") || locationKey.Contains("downtown")) return 0;
            if (locationKey.Contains("north")) return 1;
            if (locationKey.Contains("south")) return 2;
            if (locationKey.Contains("east")) return 3;
            if (locationKey.Contains("west")) return 4;
            return 5; // Other zones
        }

        private TimeSpan CalculateAverageResolutionTime()
        {
            var resolvedIssues = _issuesRepository.GetAllIssues()
                .Where(i => i.Status == IssueStatus.Resolved && i.ResolvedDate.HasValue)
                .ToList();

            if (!resolvedIssues.Any()) return TimeSpan.Zero;

            var totalTime = resolvedIssues.Sum(i => (i.ResolvedDate!.Value - i.SubmittedAt).TotalHours);
            return TimeSpan.FromHours(totalTime / resolvedIssues.Count);
        }

        private Dictionary<IssueCategory, int> GetCategoryDistribution()
        {
            return _issuesRepository.GetCategoryStatistics();
        }

        private Dictionary<IssuePriority, int> GetPriorityDistribution()
        {
            var distribution = new Dictionary<IssuePriority, int>();
            foreach (var priority in Enum.GetValues<IssuePriority>())
            {
                distribution[priority] = _issuesRepository.GetIssuesByPriority(priority).Count();
            }
            return distribution;
        }

        private Dictionary<string, int> GetLocationDistribution()
        {
            return _issuesRepository.GetMunicipalityStatistics();
        }

        private UserLevel CalculateUserLevel(int totalPoints)
        {
            return totalPoints switch
            {
                >= 1000 => UserLevel.Diamond,
                >= 500 => UserLevel.Platinum,
                >= 250 => UserLevel.Gold,
                >= 100 => UserLevel.Silver,
                _ => UserLevel.Bronze
            };
        }

        #endregion

        #region Event Handlers

        // ========================================
        // EVENT HANDLER METHODS - Observer Pattern Implementation
        // ========================================
        // These methods trigger events to notify subscribers of state changes
        
        /// <summary>
        /// Triggers the IssueSubmitted event to notify all subscribers
        /// Called when: A new issue is successfully submitted
        /// Subscribers: NotificationService, AnalyticsService, UI components
        /// </summary>
        /// <param name="e">IssueEventArgs containing the submitted issue</param>
        protected virtual void OnIssueSubmitted(IssueEventArgs e)
        {
            IssueSubmitted?.Invoke(this, e);
        }

        /// <summary>
        /// Triggers the IssueStatusChanged event to notify all subscribers
        /// Called when: An issue status is updated (e.g., InProgress -> Resolved)
        /// Subscribers: NotificationService, UserSessionService, UI components
        /// </summary>
        /// <param name="e">IssueEventArgs containing the updated issue</param>
        protected virtual void OnIssueStatusChanged(IssueEventArgs e)
        {
            IssueStatusChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Triggers the BadgeEarned event to notify all subscribers
        /// Called when: A user earns a new badge
        /// Subscribers: NotificationService, UserSessionService, UI components
        /// </summary>
        /// <param name="e">BadgeEarnedEventArgs containing the earned badge and user info</param>
        protected virtual void OnBadgeEarned(BadgeEarnedEventArgs e)
        {
            BadgeEarned?.Invoke(this, e);
        }

        /// <summary>
        /// Triggers the IssueResolved event to notify all subscribers
        /// Called when: An issue is marked as resolved
        /// Subscribers: NotificationService, AnalyticsService, UI components
        /// </summary>
        /// <param name="e">IssueEventArgs containing the resolved issue</param>
        protected virtual void OnIssueResolved(IssueEventArgs e)
        {
            IssueResolved?.Invoke(this, e);
        }

        #endregion
    }

    #region Event Args and Result Classes

    public class IssueEventArgs : EventArgs
    {
        public Issue Issue { get; }
        public DateTime Timestamp { get; }

        public IssueEventArgs(Issue issue)
        {
            Issue = issue;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class BadgeEarnedEventArgs : EventArgs
    {
        public Badge Badge { get; }
        public string UserEmail { get; }
        public DateTime Timestamp { get; }

        public BadgeEarnedEventArgs(Badge badge, string userEmail)
        {
            Badge = badge;
            UserEmail = userEmail;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class IssueSubmissionResult
    {
        public bool Success { get; set; }
        public Issue? Issue { get; set; }
        public int PointsAwarded { get; set; }
        public List<Badge> BadgesEarned { get; set; } = new List<Badge>();
        public List<Notification> NotificationsCreated { get; set; } = new List<Notification>();
        public string? ErrorMessage { get; set; }
    }

    public class BadgeEarningResult
    {
        public List<Badge> BadgesEarned { get; set; } = new List<Badge>();
        public List<Notification> NotificationsCreated { get; set; } = new List<Notification>();
    }

    public class IssueAnalytics
    {
        public int[,] CategoryPriorityMatrix { get; set; } = new int[0, 0];
        public int[,,] LocationCategoryPriorityMatrix { get; set; } = new int[0, 0, 0];
        public int TotalIssues { get; set; }
        public int ResolvedIssues { get; set; }
        public TimeSpan AverageResolutionTime { get; set; }
        public Dictionary<IssueCategory, int> CategoryDistribution { get; set; } = new Dictionary<IssueCategory, int>();
        public Dictionary<IssuePriority, int> PriorityDistribution { get; set; } = new Dictionary<IssuePriority, int>();
        public Dictionary<string, int> LocationDistribution { get; set; } = new Dictionary<string, int>();
    }

    #endregion
}
