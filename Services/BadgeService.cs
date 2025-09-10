using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Services
{
    public class BadgeService
    {
        private readonly IssuesRepository _issuesRepository;

        public BadgeService(IssuesRepository issuesRepository)
        {
            _issuesRepository = issuesRepository;
        }

        public List<Badge> GetEarnedBadges()
        {
            return _issuesRepository.GetEarnedBadges();
        }

        public List<Badge> GetAllAvailableBadges()
        {
            return _issuesRepository.GetAllAvailableBadges();
        }

        public BadgeStats GetBadgeStats()
        {
            var earnedBadges = GetEarnedBadges();
            var allBadges = GetAllAvailableBadges();

            return new BadgeStats
            {
                TotalBadges = allBadges.Count,
                EarnedBadges = earnedBadges.Count,
                LockedBadges = allBadges.Count - earnedBadges.Count,
                TotalPointsFromBadges = earnedBadges.Sum(b => b.PointsValue),
                RecentBadges = earnedBadges.OrderByDescending(b => b.EarnedDate).Take(3).ToList()
            };
        }

        public List<Badge> GetBadgesByType(BadgeType type)
        {
            return GetAllAvailableBadges().Where(b => b.Type == type).ToList();
        }

        public Badge? GetBadgeById(int id)
        {
            return GetAllAvailableBadges().FirstOrDefault(b => b.Id == id);
        }

        public List<Badge> GetEarnedBadgesByCategory(string category)
        {
            return GetEarnedBadges().Where(b => b.RequiredCategory == category).ToList();
        }
    }

    public class BadgeStats
    {
        public int TotalBadges { get; set; }
        public int EarnedBadges { get; set; }
        public int LockedBadges { get; set; }
        public int TotalPointsFromBadges { get; set; }
        public List<Badge> RecentBadges { get; set; } = new List<Badge>();
        public double CompletionPercentage => TotalBadges > 0 ? (double)EarnedBadges / TotalBadges * 100 : 0;
    }
}
