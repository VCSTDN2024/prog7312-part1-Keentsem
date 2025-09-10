using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    public class DataService
    {
        private readonly IssuesRepository _issuesRepository;
        
        // Replace basic List with advanced municipal repository
        private static readonly MunicipalRepository<UserSession> _municipalUserSessions = new MunicipalRepository<UserSession>();

        public DataService(IssuesRepository issuesRepository)
        {
            _issuesRepository = issuesRepository;
        }

        /// <summary>
        /// Get or create user session using advanced municipal repository
        /// Integrates with municipal hierarchy and resource tracking
        /// </summary>
        public UserSession GetOrCreateUserSession(string email, string name = "", string municipality = "")
        {
            var session = _municipalUserSessions.Find(s => s.UserEmail == email).FirstOrDefault();
            if (session == null)
            {
                session = new UserSession
                {
                    UserEmail = email,
                    UserName = name,
                    Municipality = municipality,
                    Province = ExtractProvince(municipality),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _municipalUserSessions.Add(session);
            }
            else
            {
                session.LastActiveAt = DateTime.UtcNow;
                session.LastModified = DateTime.UtcNow;
                _municipalUserSessions.Update(session);
            }
            return session;
        }

        public void UpdateUserSession(UserSession session)
        {
            _municipalUserSessions.Update(session);
        }

        public List<Badge> GetUserBadges(string email)
        {
            var session = _municipalUserSessions.Find(s => s.UserEmail == email).FirstOrDefault();
            if (session == null) return new List<Badge>();

            var userIssues = _issuesRepository.GetAllIssues()
                .Where(i => i.UserEmail == email)
                .ToList();

            return CalculateUserBadgesRecursively(userIssues, session, 0);
        }

        /// <summary>
        /// Recursive badge calculation through municipal department hierarchy
        /// Replaces simple counting with recursive municipal department achievement system
        /// </summary>
        private List<Badge> CalculateUserBadgesRecursively(List<Issue> userIssues, UserSession session, int departmentLevel)
        {
            var badges = new List<Badge>();

            // Base case: processed all department levels
            if (departmentLevel >= Enum.GetValues<IssueCategory>().Length)
            {
                // Add cross-department badges
                AddCrossDepartmentBadges(badges, userIssues);
                return badges;
            }

            var currentCategory = (IssueCategory)departmentLevel;
            var categoryIssues = userIssues.Where(i => i.Category == currentCategory).ToList();

            // Add department-specific badge if qualified
            if (categoryIssues.Count >= 2)
            {
                badges.Add(new Badge
                {
                    Id = 100 + departmentLevel,
                    Name = $"{currentCategory} Department Champion",
                    Description = $"Contributed significantly to {currentCategory} municipal services",
                    Type = BadgeType.CategorySpecialist,
                    IsEarned = true,
                    EarnedDate = categoryIssues.OrderBy(i => i.SubmittedAt).Skip(1).First().SubmittedAt,
                    PointsValue = CalculateDepartmentPoints(categoryIssues.Count, departmentLevel),
                    ImagePath = GetCategoryBadgeImage(currentCategory)
                });
            }

            // Recursive call for next department level
            var nextLevelBadges = CalculateUserBadgesRecursively(userIssues, session, departmentLevel + 1);
            badges.AddRange(nextLevelBadges);

            return badges;
        }

        public BadgeStats GetUserBadgeStats(string email)
        {
            var badges = GetUserBadges(email);
            var earnedBadges = badges.Where(b => b.IsEarned).ToList();
            var allBadges = _issuesRepository.GetAllAvailableBadges();

            return new BadgeStats
            {
                TotalBadges = allBadges.Count,
                EarnedBadges = earnedBadges.Count,
                LockedBadges = allBadges.Count - earnedBadges.Count,
                TotalPointsFromBadges = earnedBadges.Sum(b => b.PointsValue),
                RecentBadges = earnedBadges.OrderByDescending(b => b.EarnedDate).Take(3).ToList()
            };
        }

        public void ProcessNewIssue(Issue issue)
        {
            // Calculate points for the issue
            issue.UserPoints = CalculateIssuePoints(issue);
            
            // Add issue to repository
            _issuesRepository.AddIssue(issue);

            // Get or create user session
            var session = GetOrCreateUserSession(issue.UserEmail ?? "anonymous@demo.com", "Demo User", "Cape Town");
            
            // Update user session
            session.ReportsSubmitted++;
            session.TotalPoints += issue.UserPoints;
            session.ReportedIssues.Add(issue.Id);
            session.LastActiveAt = DateTime.UtcNow;

            // Update user level
            session.Level = CalculateUserLevel(session.TotalPoints);

            // Get user's issues for badge calculation
            var userIssues = _issuesRepository.GetAllIssues()
                .Where(i => i.UserEmail == session.UserEmail)
                .ToList();

            // Check for new badges using the proper method
            var newBadges = CalculateUserBadges(userIssues, session);
            session.EarnedBadges = newBadges.Where(b => b.IsEarned).Select(b => b.Id).ToList();

            UpdateUserSession(session);
        }

        private List<Badge> CalculateUserBadges(List<Issue> userIssues, UserSession session)
        {
            var badges = new List<Badge>();

            // First Report Badge
            if (userIssues.Any())
            {
                badges.Add(new Badge
                {
                    Id = 1,
                    Name = "First Responder",
                    Description = "Submitted your first municipal issue report",
                    ImagePath = "/images/badges/first_responder.png",
                    Type = BadgeType.FirstReport,
                    IsEarned = true,
                    EarnedDate = userIssues.OrderBy(i => i.SubmittedAt).First().SubmittedAt,
                    PointsValue = 25
                });
            }

            // Community Helper Badge (3+ reports)
            if (userIssues.Count >= 3)
            {
                badges.Add(new Badge
                {
                    Id = 2,
                    Name = "Community Helper",
                    Description = "Reported 3 or more municipal issues",
                    ImagePath = "/images/badges/community_helper.png",
                    Type = BadgeType.CommunityHelper,
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = userIssues.OrderBy(i => i.SubmittedAt).Skip(2).First().SubmittedAt,
                    PointsValue = 50
                });
            }

            // Media Contributor Badge
            if (userIssues.Any(i => i.AttachmentPaths.Any()))
            {
                badges.Add(new Badge
                {
                    Id = 3,
                    Name = "Media Contributor",
                    Description = "Submitted reports with photos or videos",
                    ImagePath = "/images/badges/neighborhood_watcher.png",
                    Type = BadgeType.MediaContributor,
                    IsEarned = true,
                    EarnedDate = userIssues.Where(i => i.AttachmentPaths.Any()).OrderBy(i => i.SubmittedAt).First().SubmittedAt,
                    PointsValue = 40
                });
            }

            // Emergency Responder Badge
            if (userIssues.Any(i => i.Priority == IssuePriority.Critical))
            {
                badges.Add(new Badge
                {
                    Id = 4,
                    Name = "Emergency Responder",
                    Description = "Reported critical priority issues",
                    ImagePath = "/images/badges/safety_sentinel.png",
                    Type = BadgeType.EmergencyResponder,
                    IsEarned = true,
                    EarnedDate = userIssues.Where(i => i.Priority == IssuePriority.Critical).OrderBy(i => i.SubmittedAt).First().SubmittedAt,
                    PointsValue = 75
                });
            }

            // Category Specialist Badges
            var categoryGroups = userIssues.GroupBy(i => i.Category);
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

            // Consistent Reporter Badge (5+ reports)
            if (userIssues.Count >= 5)
            {
                badges.Add(new Badge
                {
                    Id = 5,
                    Name = "Consistent Reporter",
                    Description = "Reported 5 or more municipal issues",
                    ImagePath = "/images/badges/persistent_reporter.png",
                    Type = BadgeType.ConsistentReporter,
                    RequiredCount = 5,
                    IsEarned = true,
                    EarnedDate = userIssues.OrderBy(i => i.SubmittedAt).Skip(4).First().SubmittedAt,
                    PointsValue = 100
                });
            }

            // Community Champion Badge (10+ reports)
            if (userIssues.Count >= 10)
            {
                badges.Add(new Badge
                {
                    Id = 6,
                    Name = "Community Champion",
                    Description = "Reported 10 or more municipal issues",
                    ImagePath = "/images/badges/community_champion.png",
                    Type = BadgeType.CommunityChampion,
                    RequiredCount = 10,
                    IsEarned = true,
                    EarnedDate = userIssues.OrderBy(i => i.SubmittedAt).Skip(9).First().SubmittedAt,
                    PointsValue = 200
                });
            }

            // Water Saver Badge (3+ water supply issues)
            var waterIssues = userIssues.Where(i => i.Category == IssueCategory.WaterSupply).ToList();
            if (waterIssues.Count >= 3)
            {
                badges.Add(new Badge
                {
                    Id = 7,
                    Name = "Water Saver",
                    Description = "Reported 3 or more water supply issues",
                    ImagePath = "/images/badges/water_saver.png",
                    Type = BadgeType.CategorySpecialist,
                    RequiredCategory = "WaterSupply",
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = waterIssues.OrderBy(i => i.SubmittedAt).Skip(2).First().SubmittedAt,
                    PointsValue = 60
                });
            }

            // Power Saver Badge (3+ electricity issues)
            var electricityIssues = userIssues.Where(i => i.Category == IssueCategory.Electricity).ToList();
            if (electricityIssues.Count >= 3)
            {
                badges.Add(new Badge
                {
                    Id = 8,
                    Name = "Power Saver",
                    Description = "Reported 3 or more electricity issues",
                    ImagePath = "/images/badges/power_saver.png",
                    Type = BadgeType.CategorySpecialist,
                    RequiredCategory = "Electricity",
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = electricityIssues.OrderBy(i => i.SubmittedAt).Skip(2).First().SubmittedAt,
                    PointsValue = 60
                });
            }

            // Road Warrior Badge (3+ roads issues)
            var roadsIssues = userIssues.Where(i => i.Category == IssueCategory.Roads).ToList();
            if (roadsIssues.Count >= 3)
            {
                badges.Add(new Badge
                {
                    Id = 9,
                    Name = "Road Warrior",
                    Description = "Reported 3 or more roads and infrastructure issues",
                    ImagePath = "/images/badges/road_warrior.png",
                    Type = BadgeType.CategorySpecialist,
                    RequiredCategory = "Roads",
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = roadsIssues.OrderBy(i => i.SubmittedAt).Skip(2).First().SubmittedAt,
                    PointsValue = 60
                });
            }

            // Eco Guardian Badge (3+ waste management or parks issues)
            var ecoIssues = userIssues.Where(i => i.Category == IssueCategory.WasteManagement || i.Category == IssueCategory.ParksAndRecreation).ToList();
            if (ecoIssues.Count >= 3)
            {
                badges.Add(new Badge
                {
                    Id = 10,
                    Name = "Eco Guardian",
                    Description = "Reported 3 or more environmental issues",
                    ImagePath = "/images/badges/eco_guardian.png",
                    Type = BadgeType.CategorySpecialist,
                    RequiredCategory = "Environmental",
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = ecoIssues.OrderBy(i => i.SubmittedAt).Skip(2).First().SubmittedAt,
                    PointsValue = 70
                });
            }

            return badges.OrderBy(b => b.EarnedDate).ToList();
        }

        private int CalculateIssuePoints(Issue issue)
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

        private string ExtractProvince(string municipality)
        {
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

        /// <summary>
        /// Add cross-department badges using operator overloading for point calculations
        /// </summary>
        private void AddCrossDepartmentBadges(List<Badge> badges, List<Issue> userIssues)
        {
            // First Report Badge
            if (userIssues.Any())
            {
                badges.Add(new Badge
                {
                    Id = 1,
                    Name = "First Responder",
                    Description = "Submitted your first municipal issue report",
                    ImagePath = "/images/badges/first_responder.png",
                    Type = BadgeType.FirstReport,
                    IsEarned = true,
                    EarnedDate = userIssues.OrderBy(i => i.SubmittedAt).First().SubmittedAt,
                    PointsValue = 25
                });
            }

            // Community Helper Badge (3+ reports)
            if (userIssues.Count >= 3)
            {
                badges.Add(new Badge
                {
                    Id = 2,
                    Name = "Community Helper",
                    Description = "Reported 3 or more municipal issues",
                    ImagePath = "/images/badges/community_helper.png",
                    Type = BadgeType.CommunityHelper,
                    RequiredCount = 3,
                    IsEarned = true,
                    EarnedDate = userIssues.OrderBy(i => i.SubmittedAt).Skip(2).First().SubmittedAt,
                    PointsValue = 50
                });
            }

            // Consistent Reporter Badge (5+ reports)
            if (userIssues.Count >= 5)
            {
                badges.Add(new Badge
                {
                    Id = 5,
                    Name = "Consistent Reporter",
                    Description = "Reported 5 or more municipal issues",
                    ImagePath = "/images/badges/persistent_reporter.png",
                    Type = BadgeType.ConsistentReporter,
                    RequiredCount = 5,
                    IsEarned = true,
                    EarnedDate = userIssues.OrderBy(i => i.SubmittedAt).Skip(4).First().SubmittedAt,
                    PointsValue = 100
                });
            }
        }

        /// <summary>
        /// Calculate department-specific points using municipal hierarchy
        /// </summary>
        private int CalculateDepartmentPoints(int issueCount, int departmentLevel)
        {
            // Use operator overloading for point calculation
            var baseIssue = new Issue { UserPoints = 25 }; // Base points per department
            var multipliedIssue = baseIssue * (departmentLevel + 1); // Department complexity multiplier
            return multipliedIssue.UserPoints * issueCount;
        }
    }
}
