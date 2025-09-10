using MunicipalServicesApp.Models;
using System.Diagnostics;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Performance Analyzer Service for Learning Unit 1
    /// Demonstrates array vs list performance analysis and algorithm complexity
    /// </summary>
    public class PerformanceAnalyzer
    {
        private readonly Random _random = new Random();

        /// <summary>
        /// Compare performance between arrays and lists for municipal data operations
        /// Demonstrates when to use arrays vs lists based on performance characteristics
        /// </summary>
        public PerformanceComparisonResult CompareDataStructures(List<Issue> issues)
        {
            var result = new PerformanceComparisonResult();
            
            // Convert to array for comparison
            var issueArray = issues.ToArray();
            
            // Test sequential access performance
            result.ArraySequentialAccessTime = MeasureSequentialAccess(issueArray);
            result.ListSequentialAccessTime = MeasureSequentialAccess(issues);
            
            // Test random access performance
            result.ArrayRandomAccessTime = MeasureRandomAccess(issueArray);
            result.ListRandomAccessTime = MeasureRandomAccess(issues);
            
            // Test insertion performance
            result.ArrayInsertionTime = MeasureInsertionPerformance(issueArray);
            result.ListInsertionTime = MeasureInsertionPerformance(issues);
            
            // Test search performance
            result.ArraySearchTime = MeasureSearchPerformance(issueArray);
            result.ListSearchTime = MeasureSearchPerformance(issues);
            
            // Memory usage analysis
            result.ArrayMemoryUsage = MeasureMemoryUsage(issueArray);
            result.ListMemoryUsage = MeasureMemoryUsage(issues);
            
            // Generate recommendations
            result.Recommendations = GenerateRecommendations(result);
            
            return result;
        }

        /// <summary>
        /// Analyze algorithm complexity for different sorting algorithms
        /// Demonstrates time and space complexity analysis
        /// </summary>
        public AlgorithmComplexityAnalysis AnalyzeComplexity()
        {
            return new AlgorithmComplexityAnalysis
            {
                QuickSort = new AlgorithmInfo
                {
                    Name = "QuickSort",
                    TimeComplexity = "O(n log n) average, O(n²) worst case",
                    SpaceComplexity = "O(log n)",
                    BestCase = "O(n log n) - balanced partitions",
                    WorstCase = "O(n²) - already sorted or reverse sorted",
                    Stable = false,
                    InPlace = true,
                    UseCase = "General purpose sorting, fastest average case"
                },
                MergeSort = new AlgorithmInfo
                {
                    Name = "MergeSort",
                    TimeComplexity = "O(n log n) guaranteed",
                    SpaceComplexity = "O(n)",
                    BestCase = "O(n log n)",
                    WorstCase = "O(n log n)",
                    Stable = true,
                    InPlace = false,
                    UseCase = "Stable sorting required, guaranteed performance"
                },
                HeapSort = new AlgorithmInfo
                {
                    Name = "HeapSort",
                    TimeComplexity = "O(n log n) guaranteed",
                    SpaceComplexity = "O(1)",
                    BestCase = "O(n log n)",
                    WorstCase = "O(n log n)",
                    Stable = false,
                    InPlace = true,
                    UseCase = "Memory constrained environments"
                },
                InsertionSort = new AlgorithmInfo
                {
                    Name = "InsertionSort",
                    TimeComplexity = "O(n²)",
                    SpaceComplexity = "O(1)",
                    BestCase = "O(n) - already sorted",
                    WorstCase = "O(n²) - reverse sorted",
                    Stable = true,
                    InPlace = true,
                    UseCase = "Small datasets, nearly sorted data"
                }
            };
        }

        /// <summary>
        /// Measure sequential access performance
        /// </summary>
        private long MeasureSequentialAccess(Issue[] array)
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < array.Length; i++)
            {
                var issue = array[i]; // Direct array access
            }
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measure sequential access performance
        /// </summary>
        private long MeasureSequentialAccess(List<Issue> list)
        {
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < list.Count; i++)
            {
                var issue = list[i]; // List indexer access
            }
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measure random access performance
        /// </summary>
        private long MeasureRandomAccess(Issue[] array)
        {
            if (array.Length == 0) return 0;
            
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < Math.Min(1000, array.Length); i++)
            {
                int randomIndex = _random.Next(0, array.Length);
                var issue = array[randomIndex];
            }
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measure random access performance
        /// </summary>
        private long MeasureRandomAccess(List<Issue> list)
        {
            if (list.Count == 0) return 0;
            
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < Math.Min(1000, list.Count); i++)
            {
                int randomIndex = _random.Next(0, list.Count);
                var issue = list[randomIndex];
            }
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measure insertion performance
        /// </summary>
        private long MeasureInsertionPerformance(Issue[] array)
        {
            var stopwatch = Stopwatch.StartNew();
            var tempList = new List<Issue>(array);
            
            // Insert at random positions
            for (int i = 0; i < Math.Min(100, array.Length); i++)
            {
                var newIssue = CreateTestIssue();
                int insertIndex = _random.Next(0, tempList.Count);
                tempList.Insert(insertIndex, newIssue);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measure insertion performance
        /// </summary>
        private long MeasureInsertionPerformance(List<Issue> list)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Insert at random positions
            for (int i = 0; i < Math.Min(100, list.Count); i++)
            {
                var newIssue = CreateTestIssue();
                int insertIndex = _random.Next(0, list.Count);
                list.Insert(insertIndex, newIssue);
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measure search performance
        /// </summary>
        private long MeasureSearchPerformance(Issue[] array)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Linear search
            for (int i = 0; i < Math.Min(100, array.Length); i++)
            {
                var targetPriority = (IssuePriority)_random.Next(1, 5);
                for (int j = 0; j < array.Length; j++)
                {
                    if (array[j].Priority == targetPriority)
                        break;
                }
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measure search performance
        /// </summary>
        private long MeasureSearchPerformance(List<Issue> list)
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Linear search
            for (int i = 0; i < Math.Min(100, list.Count); i++)
            {
                var targetPriority = (IssuePriority)_random.Next(1, 5);
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[j].Priority == targetPriority)
                        break;
                }
            }
            
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        /// <summary>
        /// Measure memory usage
        /// </summary>
        private long MeasureMemoryUsage(Issue[] array)
        {
            var beforeMemory = GC.GetTotalMemory(false);
            var tempArray = new Issue[array.Length];
            Array.Copy(array, tempArray, array.Length);
            var afterMemory = GC.GetTotalMemory(false);
            return afterMemory - beforeMemory;
        }

        /// <summary>
        /// Measure memory usage
        /// </summary>
        private long MeasureMemoryUsage(List<Issue> list)
        {
            var beforeMemory = GC.GetTotalMemory(false);
            var tempList = new List<Issue>(list);
            var afterMemory = GC.GetTotalMemory(false);
            return afterMemory - beforeMemory;
        }

        /// <summary>
        /// Generate performance recommendations
        /// </summary>
        private List<string> GenerateRecommendations(PerformanceComparisonResult result)
        {
            var recommendations = new List<string>();
            
            if (result.ArraySequentialAccessTime < result.ListSequentialAccessTime)
            {
                recommendations.Add("Use arrays for sequential access operations - faster direct memory access");
            }
            else
            {
                recommendations.Add("Use lists for sequential access operations - better performance with bounds checking");
            }
            
            if (result.ArrayRandomAccessTime < result.ListRandomAccessTime)
            {
                recommendations.Add("Use arrays for random access operations - direct memory indexing");
            }
            else
            {
                recommendations.Add("Use lists for random access operations - optimized indexer performance");
            }
            
            if (result.ListInsertionTime < result.ArrayInsertionTime)
            {
                recommendations.Add("Use lists for frequent insertions - built-in dynamic resizing");
            }
            else
            {
                recommendations.Add("Use arrays for insertion operations - fixed size prevents memory fragmentation");
            }
            
            if (result.ArrayMemoryUsage < result.ListMemoryUsage)
            {
                recommendations.Add("Use arrays for memory efficiency - lower overhead per element");
            }
            else
            {
                recommendations.Add("Use lists for memory efficiency - better memory management");
            }
            
            recommendations.Add("For municipal services: Use arrays for read-heavy operations, lists for dynamic data");
            recommendations.Add("For performance-critical operations: Use arrays with direct memory access");
            recommendations.Add("For data that changes frequently: Use lists with built-in growth management");
            
            return recommendations;
        }

        /// <summary>
        /// Create a test issue for performance testing
        /// </summary>
        private Issue CreateTestIssue()
        {
            return new Issue
            {
                Title = $"Test Issue {_random.Next(1000, 9999)}",
                Description = "Performance test issue",
                Category = (IssueCategory)_random.Next(0, 8),
                Priority = (IssuePriority)_random.Next(1, 5),
                Location = $"Test Location {_random.Next(1, 100)}",
                UserPoints = _random.Next(10, 100)
            };
        }
    }

    /// <summary>
    /// Performance comparison result for arrays vs lists
    /// </summary>
    public class PerformanceComparisonResult
    {
        public long ArraySequentialAccessTime { get; set; }
        public long ListSequentialAccessTime { get; set; }
        public long ArrayRandomAccessTime { get; set; }
        public long ListRandomAccessTime { get; set; }
        public long ArrayInsertionTime { get; set; }
        public long ListInsertionTime { get; set; }
        public long ArraySearchTime { get; set; }
        public long ListSearchTime { get; set; }
        public long ArrayMemoryUsage { get; set; }
        public long ListMemoryUsage { get; set; }
        public List<string> Recommendations { get; set; } = new List<string>();
    }

    /// <summary>
    /// Algorithm complexity analysis result
    /// </summary>
    public class AlgorithmComplexityAnalysis
    {
        public AlgorithmInfo QuickSort { get; set; } = new AlgorithmInfo();
        public AlgorithmInfo MergeSort { get; set; } = new AlgorithmInfo();
        public AlgorithmInfo HeapSort { get; set; } = new AlgorithmInfo();
        public AlgorithmInfo InsertionSort { get; set; } = new AlgorithmInfo();
    }

    /// <summary>
    /// Information about a specific algorithm
    /// </summary>
    public class AlgorithmInfo
    {
        public string Name { get; set; } = string.Empty;
        public string TimeComplexity { get; set; } = string.Empty;
        public string SpaceComplexity { get; set; } = string.Empty;
        public string BestCase { get; set; } = string.Empty;
        public string WorstCase { get; set; } = string.Empty;
        public bool Stable { get; set; }
        public bool InPlace { get; set; }
        public string UseCase { get; set; } = string.Empty;
    }
}