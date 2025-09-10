using Microsoft.AspNetCore.Mvc.RazorPages;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;

namespace MunicipalServicesApp.Pages
{
    public class AnalyticsDashboardModel : PageModel
    {
        private readonly IssueTimelineService _timelineService;
        private readonly MunicipalAnalyticsService _analyticsService;

        public AnalyticsDashboardModel(IssueTimelineService timelineService, MunicipalAnalyticsService analyticsService)
        {
            _timelineService = timelineService;
            _analyticsService = analyticsService;
        }

        // Peak Reporting Data
        public DayOfWeek PeakReportingDay { get; set; }
        public int PeakReportingHour { get; set; }
        public int PeakIssueCount { get; set; }
        public int RecommendedCrewSize { get; set; }
        public double CurrentEfficiency { get; set; }

        // Workflow Bottlenecks
        public List<WorkflowBottleneck> WorkflowBottlenecks { get; set; } = new();

        // Seasonal Predictions
        public List<SeasonalPrediction> SeasonalPredictions { get; set; } = new();

        // Resolution Times
        public List<ResolutionTimeInfo> ResolutionTimes { get; set; } = new();

        // Performance Metrics
        public double[] CategorySatisfactionScores { get; set; } = Array.Empty<double>();
        public double[] PriorityEfficiencyScores { get; set; } = Array.Empty<double>();

        public void OnGet()
        {
            LoadPeakReportingData();
            LoadWorkflowBottlenecks();
            LoadSeasonalPredictions();
            LoadResolutionTimes();
            LoadPerformanceMetrics();
        }

        private void LoadPeakReportingData()
        {
            var peakTime = _timelineService.GetPeakReportingTime();
            PeakReportingDay = (DayOfWeek)peakTime.day;
            PeakReportingHour = peakTime.hour;
            PeakIssueCount = peakTime.count;

            var recommendations = _timelineService.GetResourceRecommendations();
            RecommendedCrewSize = recommendations.RecommendedCrewSize;
            CurrentEfficiency = recommendations.AverageResponseTimes.Length > 0 ? 
                recommendations.AverageResponseTimes.Average() / 24.0 : 0.0; // Convert to efficiency percentage
        }

        private void LoadWorkflowBottlenecks()
        {
            WorkflowBottlenecks = _timelineService.GetWorkflowBottlenecks();
        }

        private void LoadSeasonalPredictions()
        {
            var currentMonth = DateTime.Now.Month;
            var categories = Enum.GetValues<IssueCategory>();
            
            foreach (var category in categories)
            {
                var prediction = _analyticsService.GetSeasonalPrediction(category, currentMonth);
                SeasonalPredictions.Add(prediction);
            }
        }

        private void LoadResolutionTimes()
        {
            var categories = Enum.GetValues<IssueCategory>();
            var priorities = Enum.GetValues<IssuePriority>();

            foreach (var category in categories)
            {
                var totalResolutionTime = 0.0;
                var count = 0;

                foreach (var priority in priorities)
                {
                    var resolutionTime = _analyticsService.GetAverageResolutionTime(category, priority);
                    if (resolutionTime > 0)
                    {
                        totalResolutionTime += resolutionTime;
                        count++;
                    }
                }

                if (count > 0)
                {
                    ResolutionTimes.Add(new ResolutionTimeInfo
                    {
                        Category = category,
                        AverageHours = totalResolutionTime / count
                    });
                }
            }
        }

        private void LoadPerformanceMetrics()
        {
            var dashboard = _analyticsService.GetPerformanceDashboard();
            CategorySatisfactionScores = dashboard.CategorySatisfactionScores;
            PriorityEfficiencyScores = dashboard.PriorityEfficiencyScores;
        }
    }

    public class ResolutionTimeInfo
    {
        public IssueCategory Category { get; set; }
        public double AverageHours { get; set; }
    }
}
