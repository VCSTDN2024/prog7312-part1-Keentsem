using MunicipalServicesApp.Models;
using System.Collections.Generic;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Practical Municipal Analytics Service using arrays for real municipal insights
    /// Replaces academic demos with meaningful municipal data analysis
    /// </summary>
    public class MunicipalAnalyticsService
    {
        // 2D Array: [Month][Category] - Track seasonal issue patterns
        private static int[,] _seasonalIssuePatterns = new int[12, 8]; // 12 months, 8 categories
        
        // 3D Array: [Priority][Category][LocationZone] - Resource allocation matrix
        private static int[,,] _resourceAllocationMatrix = new int[4, 8, 5]; // 4 priorities, 8 categories, 5 location zones
        
        // Jagged Array: Issue escalation patterns by category
        private static List<IssueEscalation>[][] _escalationPatterns = new List<IssueEscalation>[8][];
        
        // Array for tracking citizen satisfaction by category
        private static double[] _categorySatisfactionScores = new double[8];
        
        // Array for tracking resolution efficiency by priority
        private static double[] _priorityEfficiencyScores = new double[4];

        public MunicipalAnalyticsService()
        {
            InitializeAnalyticsStructures();
        }

        private void InitializeAnalyticsStructures()
        {
            // Initialize jagged array for escalation patterns
            for (int i = 0; i < _escalationPatterns.Length; i++)
            {
                _escalationPatterns[i] = new List<IssueEscalation>[4]; // 4 priority levels
                for (int j = 0; j < 4; j++)
                {
                    _escalationPatterns[i][j] = new List<IssueEscalation>();
                }
            }
        }

        /// <summary>
        /// Analyze issue patterns for municipal planning
        /// </summary>
        public void AnalyzeIssuePatterns(Issue issue)
        {
            var month = issue.SubmittedAt.Month - 1; // Convert to 0-based index
            var categoryIndex = (int)issue.Category;
            var priorityIndex = (int)issue.Priority;
            var locationZone = GetLocationZone(issue.Location);
            
            // Track seasonal patterns
            _seasonalIssuePatterns[month, categoryIndex]++;
            
            // Track resource allocation needs
            _resourceAllocationMatrix[priorityIndex, categoryIndex, locationZone]++;
        }

        /// <summary>
        /// Track issue escalations for performance monitoring
        /// </summary>
        public void TrackEscalation(Issue issue, IssuePriority fromPriority, IssuePriority toPriority, string reason)
        {
            var categoryIndex = (int)issue.Category;
            var fromPriorityIndex = (int)fromPriority;
            
            var escalation = new IssueEscalation
            {
                IssueId = issue.Id,
                Category = issue.Category,
                FromPriority = fromPriority,
                ToPriority = toPriority,
                EscalationReason = reason,
                EscalatedAt = DateTime.UtcNow,
                DaysSinceReported = (DateTime.UtcNow - issue.SubmittedAt).TotalDays
            };
            
            _escalationPatterns[categoryIndex][fromPriorityIndex].Add(escalation);
        }

        /// <summary>
        /// Update satisfaction scores based on resolution feedback
        /// </summary>
        public void UpdateSatisfactionScore(IssueCategory category, double satisfactionScore)
        {
            var categoryIndex = (int)category;
            
            // Simple moving average for satisfaction scores
            _categorySatisfactionScores[categoryIndex] = 
                (_categorySatisfactionScores[categoryIndex] * 0.8) + (satisfactionScore * 0.2);
        }

        /// <summary>
        /// Get seasonal issue predictions for resource planning
        /// </summary>
        public SeasonalPrediction GetSeasonalPrediction(IssueCategory category, int month)
        {
            var categoryIndex = (int)category;
            var monthIndex = month - 1;
            
            // Calculate average for this month over the year
            var currentMonthAverage = _seasonalIssuePatterns[monthIndex, categoryIndex];
            
            // Calculate trend from previous months
            var previousMonths = new List<int>();
            for (int i = 0; i < monthIndex; i++)
            {
                previousMonths.Add(_seasonalIssuePatterns[i, categoryIndex]);
            }
            
            var trend = previousMonths.Count > 0 ? 
                (currentMonthAverage - previousMonths.Average()) / previousMonths.Average() : 0;
            
            return new SeasonalPrediction
            {
                Category = category,
                Month = month,
                PredictedVolume = (int)(currentMonthAverage * (1 + trend)),
                Trend = trend > 0.1 ? "Increasing" : trend < -0.1 ? "Decreasing" : "Stable",
                Confidence = CalculateConfidence(monthIndex, categoryIndex)
            };
        }

        /// <summary>
        /// Get resource allocation recommendations
        /// </summary>
        public ResourceAllocationReport GetResourceAllocationReport()
        {
            var report = new ResourceAllocationReport();
            
            // Analyze resource needs by priority and category
            for (int priority = 0; priority < 4; priority++)
            {
                for (int category = 0; category < 8; category++)
                {
                    var totalIssues = 0;
                    for (int zone = 0; zone < 5; zone++)
                    {
                        totalIssues += _resourceAllocationMatrix[priority, category, zone];
                    }
                    
                    if (totalIssues > 0)
                    {
                        report.CategoryAllocations.Add(new CategoryAllocation
                        {
                            Category = (IssueCategory)category,
                            Priority = (IssuePriority)priority,
                            TotalIssues = totalIssues,
                            RecommendedResources = CalculateRecommendedResources(totalIssues, priority),
                            EfficiencyScore = _priorityEfficiencyScores[priority]
                        });
                    }
                }
            }
            
            return report;
        }

        /// <summary>
        /// Get average resolution time for a specific category and priority
        /// </summary>
        public double GetAverageResolutionTime(IssueCategory category, IssuePriority priority)
        {
            var categoryIndex = (int)category;
            var priorityIndex = (int)priority;
            
            // This would need to be implemented with actual resolution data
            // For now, return a placeholder based on priority
            return priority switch
            {
                IssuePriority.Critical => 4.0, // 4 hours
                IssuePriority.High => 12.0,    // 12 hours
                IssuePriority.Medium => 48.0,  // 2 days
                IssuePriority.Low => 120.0,    // 5 days
                _ => 48.0
            };
        }

        /// <summary>
        /// Get escalation analysis for performance improvement
        /// </summary>
        public EscalationAnalysis GetEscalationAnalysis()
        {
            var analysis = new EscalationAnalysis();
            
            for (int category = 0; category < 8; category++)
            {
                for (int priority = 0; priority < 4; priority++)
                {
                    var escalations = _escalationPatterns[category][priority];
                    if (escalations.Count > 0)
                    {
                        analysis.CategoryEscalations.Add(new CategoryEscalation
                        {
                            Category = (IssueCategory)category,
                            Priority = (IssuePriority)priority,
                            EscalationCount = escalations.Count,
                            AverageDaysToEscalate = escalations.Average(e => e.DaysSinceReported),
                            CommonReasons = GetCommonEscalationReasons(escalations)
                        });
                    }
                }
            }
            
            return analysis;
        }

        /// <summary>
        /// Get municipal performance dashboard data
        /// </summary>
        public MunicipalPerformanceDashboard GetPerformanceDashboard()
        {
            return new MunicipalPerformanceDashboard
            {
                CategorySatisfactionScores = _categorySatisfactionScores.ToArray(),
                PriorityEfficiencyScores = _priorityEfficiencyScores.ToArray(),
                SeasonalPatterns = GetSeasonalPatternsSummary(),
                ResourceUtilization = GetResourceUtilizationSummary(),
                GeneratedAt = DateTime.UtcNow
            };
        }

        private int GetLocationZone(string location)
        {
            // Simple zone mapping based on location keywords
            var lowerLocation = location.ToLower();
            if (lowerLocation.Contains("city") || lowerLocation.Contains("central")) return 0;
            if (lowerLocation.Contains("north")) return 1;
            if (lowerLocation.Contains("south")) return 2;
            if (lowerLocation.Contains("east")) return 3;
            return 4; // West or other
        }

        private int CalculateRecommendedResources(int issueCount, int priority)
        {
            // Higher priority issues need more resources
            var baseResources = issueCount / 10;
            var priorityMultiplier = priority + 1; // 1-4
            return Math.Max(1, baseResources * priorityMultiplier);
        }

        private double CalculateConfidence(int monthIndex, int categoryIndex)
        {
            // Simple confidence calculation based on data availability
            var dataPoints = _seasonalIssuePatterns[monthIndex, categoryIndex];
            return Math.Min(1.0, dataPoints / 10.0); // Max confidence at 10+ data points
        }

        private List<string> GetCommonEscalationReasons(List<IssueEscalation> escalations)
        {
            return escalations
                .GroupBy(e => e.EscalationReason)
                .OrderByDescending(g => g.Count())
                .Take(3)
                .Select(g => g.Key)
                .ToList();
        }

        private Dictionary<IssueCategory, int[]> GetSeasonalPatternsSummary()
        {
            var patterns = new Dictionary<IssueCategory, int[]>();
            for (int category = 0; category < 8; category++)
            {
                var monthlyData = new int[12];
                for (int month = 0; month < 12; month++)
                {
                    monthlyData[month] = _seasonalIssuePatterns[month, category];
                }
                patterns[(IssueCategory)category] = monthlyData;
            }
            return patterns;
        }

        private Dictionary<string, double> GetResourceUtilizationSummary()
        {
            var utilization = new Dictionary<string, double>();
            var totalIssues = 0;
            var totalResources = 0;
            
            for (int priority = 0; priority < 4; priority++)
            {
                for (int category = 0; category < 8; category++)
                {
                    for (int zone = 0; zone < 5; zone++)
                    {
                        totalIssues += _resourceAllocationMatrix[priority, category, zone];
                    }
                }
            }
            
            utilization["Total Issues"] = totalIssues;
            utilization["Resource Efficiency"] = totalIssues > 0 ? (double)totalIssues / totalResources : 0;
            
            return utilization;
        }
    }

    public class IssueEscalation
    {
        public Guid IssueId { get; set; }
        public IssueCategory Category { get; set; }
        public IssuePriority FromPriority { get; set; }
        public IssuePriority ToPriority { get; set; }
        public string EscalationReason { get; set; } = string.Empty;
        public DateTime EscalatedAt { get; set; }
        public double DaysSinceReported { get; set; }
    }

    public class SeasonalPrediction
    {
        public IssueCategory Category { get; set; }
        public int Month { get; set; }
        public int PredictedVolume { get; set; }
        public string Trend { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    public class ResourceAllocationReport
    {
        public List<CategoryAllocation> CategoryAllocations { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class CategoryAllocation
    {
        public IssueCategory Category { get; set; }
        public IssuePriority Priority { get; set; }
        public int TotalIssues { get; set; }
        public int RecommendedResources { get; set; }
        public double EfficiencyScore { get; set; }
    }

    public class EscalationAnalysis
    {
        public List<CategoryEscalation> CategoryEscalations { get; set; } = new();
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class CategoryEscalation
    {
        public IssueCategory Category { get; set; }
        public IssuePriority Priority { get; set; }
        public int EscalationCount { get; set; }
        public double AverageDaysToEscalate { get; set; }
        public List<string> CommonReasons { get; set; } = new();
    }

    public class MunicipalPerformanceDashboard
    {
        public double[] CategorySatisfactionScores { get; set; } = Array.Empty<double>();
        public double[] PriorityEfficiencyScores { get; set; } = Array.Empty<double>();
        public Dictionary<IssueCategory, int[]> SeasonalPatterns { get; set; } = new();
        public Dictionary<string, double> ResourceUtilization { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }
}
