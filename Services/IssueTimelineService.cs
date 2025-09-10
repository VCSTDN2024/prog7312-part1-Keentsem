using MunicipalServicesApp.Models;
using System.Collections.Generic;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Practical Issue Timeline Service using arrays for real municipal workflow tracking
    /// Replaces academic demos with meaningful municipal operations
    /// </summary>
    public class IssueTimelineService
    {
        // 2D Array: [Day][Hour] - Track issue volume patterns for resource planning
        private static int[,] _dailyIssueVolume = new int[7, 24]; // 7 days, 24 hours
        
        // 3D Array: [Category][Priority][Status] - Track issue progression through workflow
        private static int[,,] _issueWorkflowMatrix = new int[8, 4, 4]; // 8 categories, 4 priorities, 4 statuses
        
        // Jagged Array: Issue resolution timeline by category
        private static List<IssueResolutionTime>[][] _resolutionTimelines = new List<IssueResolutionTime>[8][];
        
        // Array for tracking response times by priority
        private static double[] _averageResponseTimes = new double[4]; // One for each priority level
        
        public IssueTimelineService()
        {
            InitializeTimelineStructures();
        }

        private void InitializeTimelineStructures()
        {
            // Initialize jagged array for resolution timelines
            for (int i = 0; i < _resolutionTimelines.Length; i++)
            {
                _resolutionTimelines[i] = new List<IssueResolutionTime>[4]; // 4 priority levels
                for (int j = 0; j < 4; j++)
                {
                    _resolutionTimelines[i][j] = new List<IssueResolutionTime>();
                }
            }
        }

        /// <summary>
        /// Log when an issue is reported - tracks patterns for resource allocation
        /// </summary>
        public void LogIssueReported(Issue issue)
        {
            var dayOfWeek = (int)issue.SubmittedAt.DayOfWeek;
            var hour = issue.SubmittedAt.Hour;
            
            // Track daily/hourly patterns for crew scheduling
            _dailyIssueVolume[dayOfWeek, hour]++;
            
            // Track workflow progression
            var categoryIndex = (int)issue.Category;
            var priorityIndex = (int)issue.Priority;
            var statusIndex = (int)issue.Status;
            
            _issueWorkflowMatrix[categoryIndex, priorityIndex, statusIndex]++;
        }

        /// <summary>
        /// Log when an issue status changes - tracks workflow efficiency
        /// </summary>
        public void LogStatusChange(Issue issue, IssueStatus oldStatus, IssueStatus newStatus)
        {
            var categoryIndex = (int)issue.Category;
            var priorityIndex = (int)issue.Priority;
            
            // Decrement old status count
            _issueWorkflowMatrix[categoryIndex, priorityIndex, (int)oldStatus]--;
            // Increment new status count
            _issueWorkflowMatrix[categoryIndex, priorityIndex, (int)newStatus]++;
            
            // If resolved, track resolution time
            if (newStatus == IssueStatus.Resolved || newStatus == IssueStatus.Closed)
            {
                var resolutionTime = new IssueResolutionTime
                {
                    IssueId = issue.Id,
                    Category = issue.Category,
                    Priority = issue.Priority,
                    ResolutionTimeHours = (DateTime.UtcNow - issue.SubmittedAt).TotalHours,
                    ResolvedAt = DateTime.UtcNow
                };
                
                _resolutionTimelines[categoryIndex][priorityIndex].Add(resolutionTime);
                UpdateAverageResponseTimes();
            }
        }

        /// <summary>
        /// Get peak reporting hours for crew scheduling
        /// </summary>
        public (int day, int hour, int count) GetPeakReportingTime()
        {
            int maxCount = 0;
            int peakDay = 0;
            int peakHour = 0;
            
            for (int day = 0; day < 7; day++)
            {
                for (int hour = 0; hour < 24; hour++)
                {
                    if (_dailyIssueVolume[day, hour] > maxCount)
                    {
                        maxCount = _dailyIssueVolume[day, hour];
                        peakDay = day;
                        peakHour = hour;
                    }
                }
            }
            
            return (peakDay, peakHour, maxCount);
        }

        /// <summary>
        /// Get workflow bottlenecks - where issues get stuck
        /// </summary>
        public List<WorkflowBottleneck> GetWorkflowBottlenecks()
        {
            var bottlenecks = new List<WorkflowBottleneck>();
            
            for (int category = 0; category < 8; category++)
            {
                for (int priority = 0; priority < 4; priority++)
                {
                    // Check for high counts in "InProgress" status (potential bottleneck)
                    var inProgressCount = _issueWorkflowMatrix[category, priority, (int)IssueStatus.InProgress];
                    var openCount = _issueWorkflowMatrix[category, priority, (int)IssueStatus.Open];
                    
                    if (inProgressCount > openCount * 2) // More in progress than open
                    {
                        bottlenecks.Add(new WorkflowBottleneck
                        {
                            Category = (IssueCategory)category,
                            Priority = (IssuePriority)priority,
                            InProgressCount = inProgressCount,
                            OpenCount = openCount,
                            Severity = inProgressCount > openCount * 3 ? "High" : "Medium"
                        });
                    }
                }
            }
            
            return bottlenecks;
        }

        /// <summary>
        /// Get average resolution times by category and priority
        /// </summary>
        public double GetAverageResolutionTime(IssueCategory category, IssuePriority priority)
        {
            var categoryIndex = (int)category;
            var priorityIndex = (int)priority;
            
            var resolutions = _resolutionTimelines[categoryIndex][priorityIndex];
            if (resolutions.Count == 0) return 0;
            
            return resolutions.Average(r => r.ResolutionTimeHours);
        }

        /// <summary>
        /// Get resource allocation recommendations based on patterns
        /// </summary>
        public ResourceAllocationRecommendation GetResourceRecommendations()
        {
            var peakTime = GetPeakReportingTime();
            var bottlenecks = GetWorkflowBottlenecks();
            
            return new ResourceAllocationRecommendation
            {
                PeakReportingDay = (DayOfWeek)peakTime.day,
                PeakReportingHour = peakTime.hour,
                RecommendedCrewSize = CalculateRecommendedCrewSize(peakTime.count),
                Bottlenecks = bottlenecks,
                AverageResponseTimes = _averageResponseTimes.ToArray()
            };
        }

        private void UpdateAverageResponseTimes()
        {
            for (int priority = 0; priority < 4; priority++)
            {
                var allResolutions = new List<IssueResolutionTime>();
                for (int category = 0; category < 8; category++)
                {
                    allResolutions.AddRange(_resolutionTimelines[category][priority]);
                }
                
                if (allResolutions.Count > 0)
                {
                    _averageResponseTimes[priority] = allResolutions.Average(r => r.ResolutionTimeHours);
                }
            }
        }

        private int CalculateRecommendedCrewSize(int peakCount)
        {
            // Simple heuristic: 1 crew member per 5 issues during peak time
            return Math.Max(1, peakCount / 5);
        }
    }

    public class IssueResolutionTime
    {
        public Guid IssueId { get; set; }
        public IssueCategory Category { get; set; }
        public IssuePriority Priority { get; set; }
        public double ResolutionTimeHours { get; set; }
        public DateTime ResolvedAt { get; set; }
    }

    public class WorkflowBottleneck
    {
        public IssueCategory Category { get; set; }
        public IssuePriority Priority { get; set; }
        public int InProgressCount { get; set; }
        public int OpenCount { get; set; }
        public string Severity { get; set; } = string.Empty;
    }

    public class ResourceAllocationRecommendation
    {
        public DayOfWeek PeakReportingDay { get; set; }
        public int PeakReportingHour { get; set; }
        public int RecommendedCrewSize { get; set; }
        public List<WorkflowBottleneck> Bottlenecks { get; set; } = new();
        public double[] AverageResponseTimes { get; set; } = Array.Empty<double>();
    }
}
