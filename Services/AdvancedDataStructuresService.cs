using MunicipalServicesApp.Models;
using System.Diagnostics;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Advanced Data Structures Service implementing Learning Unit 1 & 2 requirements
    /// Demonstrates multi-dimensional arrays, jagged arrays, custom sorting, recursion, and advanced generics
    /// </summary>
    public class AdvancedDataStructuresService
    {
        // Learning Unit 1: Multi-dimensional Arrays
        private static int[,] categoryLocationMatrix = new int[8, 4]; // 2D array for category-location mapping
        private static int[,,] priorityCategoryLocationMatrix = new int[4, 8, 4]; // 3D array for priority-category-location
        
        // Learning Unit 1: Jagged Arrays
        private static string[][] municipalityHierarchy = new string[5][]; // Variable-length structure for location hierarchy
        private static List<Issue>[][] issuesByCategoryAndPriority = new List<Issue>[8][]; // Jagged array for complex data organization
        
        // Learning Unit 2: Advanced Generic Repository
        private readonly Repository<Issue> _issueRepository;
        private readonly Repository<Badge> _badgeRepository;
        
        public AdvancedDataStructuresService()
        {
            _issueRepository = new Repository<Issue>();
            _badgeRepository = new Repository<Badge>();
            InitializeDataStructures();
        }

        #region Learning Unit 1: Multi-dimensional Arrays Implementation

        /// <summary>
        /// Initialize multi-dimensional arrays for geographical data organization
        /// Demonstrates 2D and 3D array usage for complex data mapping
        /// </summary>
        private void InitializeDataStructures()
        {
            // 2D Array: Category vs Location mapping
            // Rows: IssueCategory (8 categories), Columns: Location zones (4 zones)
            categoryLocationMatrix = new int[8, 4];
            
            // 3D Array: Priority vs Category vs Location
            // Dimensions: [Priority][Category][LocationZone]
            priorityCategoryLocationMatrix = new int[4, 8, 4];
            
            // Jagged Array: Municipality hierarchy (variable length)
            municipalityHierarchy = new string[5][]
            {
                new string[] { "Western Cape", "Cape Town", "City Bowl", "Gardens" },
                new string[] { "Gauteng", "Johannesburg", "Sandton", "Rivonia" },
                new string[] { "KwaZulu-Natal", "Durban", "Umhlanga", "Gateway" },
                new string[] { "Eastern Cape", "Port Elizabeth", "Summerstrand" },
                new string[] { "Free State", "Bloemfontein", "Central" }
            };
            
            // Jagged Array: Issues organized by category and priority
            issuesByCategoryAndPriority = new List<Issue>[8][];
            for (int i = 0; i < 8; i++) // 8 categories
            {
                issuesByCategoryAndPriority[i] = new List<Issue>[4]; // 4 priority levels
                for (int j = 0; j < 4; j++)
                {
                    issuesByCategoryAndPriority[i][j] = new List<Issue>();
                }
            }
        }

        /// <summary>
        /// Populate multi-dimensional arrays with issue data
        /// Demonstrates efficient data organization using 2D/3D arrays
        /// </summary>
        public void PopulateGeographicalData(List<Issue> issues)
        {
            // Reset matrices
            Array.Clear(categoryLocationMatrix, 0, categoryLocationMatrix.Length);
            Array.Clear(priorityCategoryLocationMatrix, 0, priorityCategoryLocationMatrix.Length);
            
            // Clear jagged array
            for (int i = 0; i < issuesByCategoryAndPriority.Length; i++)
            {
                for (int j = 0; j < issuesByCategoryAndPriority[i].Length; j++)
                {
                    issuesByCategoryAndPriority[i][j].Clear();
                }
            }
            
            foreach (var issue in issues)
            {
                int categoryIndex = (int)issue.Category;
                int priorityIndex = (int)issue.Priority - 1; // Priority enum starts at 1
                int locationZone = GetLocationZone(issue.Location);
                
                // Bounds checking for array access
                if (categoryIndex >= 0 && categoryIndex < categoryLocationMatrix.GetLength(0) &&
                    locationZone >= 0 && locationZone < categoryLocationMatrix.GetLength(1))
                {
                    // Update 2D matrix
                    categoryLocationMatrix[categoryIndex, locationZone]++;
                }
                
                // Bounds checking for 3D array access
                if (priorityIndex >= 0 && priorityIndex < priorityCategoryLocationMatrix.GetLength(0) &&
                    categoryIndex >= 0 && categoryIndex < priorityCategoryLocationMatrix.GetLength(1) &&
                    locationZone >= 0 && locationZone < priorityCategoryLocationMatrix.GetLength(2))
                {
                    // Update 3D matrix
                    priorityCategoryLocationMatrix[priorityIndex, categoryIndex, locationZone]++;
                }
                
                // Update jagged array with bounds checking
                if (categoryIndex >= 0 && categoryIndex < issuesByCategoryAndPriority.Length && 
                    priorityIndex >= 0 && priorityIndex < issuesByCategoryAndPriority[categoryIndex].Length)
                {
                    issuesByCategoryAndPriority[categoryIndex][priorityIndex].Add(issue);
                }
            }
        }

        /// <summary>
        /// Get location zone from location string
        /// Demonstrates array indexing and data extraction
        /// </summary>
        private int GetLocationZone(string location)
        {
            // Simple zone mapping based on location string
            if (location.ToLower().Contains("city") || location.ToLower().Contains("downtown"))
                return 0; // City center
            else if (location.ToLower().Contains("north") || location.ToLower().Contains("northern"))
                return 1; // North zone
            else if (location.ToLower().Contains("south") || location.ToLower().Contains("southern"))
                return 2; // South zone
            else
                return 3; // Other zones
        }

        /// <summary>
        /// Get geographical statistics using multi-dimensional arrays
        /// Demonstrates data analysis with 2D/3D arrays
        /// </summary>
        public GeographicalStats GetGeographicalStats()
        {
            var stats = new GeographicalStats();
            
            // Analyze 2D matrix
            for (int category = 0; category < categoryLocationMatrix.GetLength(0); category++)
            {
                for (int zone = 0; zone < categoryLocationMatrix.GetLength(1); zone++)
                {
                    int count = categoryLocationMatrix[category, zone];
                    if (count > 0)
                    {
                        stats.CategoryZoneCounts.Add(new CategoryZoneCount
                        {
                            Category = (IssueCategory)category,
                            Zone = zone,
                            Count = count
                        });
                    }
                }
            }
            
            // Analyze 3D matrix
            for (int priority = 0; priority < priorityCategoryLocationMatrix.GetLength(0); priority++)
            {
                for (int category = 0; category < priorityCategoryLocationMatrix.GetLength(1); category++)
                {
                    for (int zone = 0; zone < priorityCategoryLocationMatrix.GetLength(2); zone++)
                    {
                        int count = priorityCategoryLocationMatrix[priority, category, zone];
                        if (count > 0)
                        {
                            stats.PriorityCategoryZoneCounts.Add(new PriorityCategoryZoneCount
                            {
                                Priority = (IssuePriority)(priority + 1),
                                Category = (IssueCategory)category,
                                Zone = zone,
                                Count = count
                            });
                        }
                    }
                }
            }
            
            return stats;
        }

        #endregion

        #region Learning Unit 1: Custom Sorting Algorithms Implementation

        /// <summary>
        /// QuickSort implementation for issues by priority
        /// Demonstrates custom sorting algorithm implementation
        /// </summary>
        public static void QuickSortIssues(Issue[] issues, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(issues, low, high);
                QuickSortIssues(issues, low, pivotIndex - 1);
                QuickSortIssues(issues, pivotIndex + 1, high);
            }
        }

        /// <summary>
        /// Partition method for QuickSort
        /// </summary>
        private static int Partition(Issue[] issues, int low, int high)
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
        /// MergeSort implementation for issues by submission date
        /// Demonstrates divide-and-conquer sorting algorithm
        /// </summary>
        public static void MergeSortByDate(List<Issue> issues)
        {
            if (issues.Count <= 1) return;
            
            Issue[] temp = new Issue[issues.Count];
            MergeSortByDateRecursive(issues, temp, 0, issues.Count - 1);
        }

        /// <summary>
        /// Recursive MergeSort implementation
        /// </summary>
        private static void MergeSortByDateRecursive(List<Issue> issues, Issue[] temp, int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                MergeSortByDateRecursive(issues, temp, left, mid);
                MergeSortByDateRecursive(issues, temp, mid + 1, right);
                Merge(issues, temp, left, mid, right);
            }
        }

        /// <summary>
        /// Merge method for MergeSort
        /// </summary>
        private static void Merge(List<Issue> issues, Issue[] temp, int left, int mid, int right)
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
        /// Demonstrates another custom sorting algorithm
        /// </summary>
        public static void HeapSortByPoints(Issue[] issues)
        {
            int n = issues.Length;

            // Build heap
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(issues, n, i);

            // Extract elements from heap
            for (int i = n - 1; i > 0; i--)
            {
                Swap(issues, 0, i);
                Heapify(issues, i, 0);
            }
        }

        /// <summary>
        /// Heapify method for HeapSort
        /// </summary>
        private static void Heapify(Issue[] issues, int n, int i)
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
                Heapify(issues, n, largest);
            }
        }

        /// <summary>
        /// Swap utility method
        /// </summary>
        private static void Swap(Issue[] issues, int i, int j)
        {
            Issue temp = issues[i];
            issues[i] = issues[j];
            issues[j] = temp;
        }

        #endregion

        #region Learning Unit 1: Array vs List Comparison

        /// <summary>
        /// Demonstrates when to use arrays vs lists
        /// Shows performance and memory characteristics
        /// </summary>
        public ArrayVsListComparison CompareArrayVsList(List<Issue> issues)
        {
            var comparison = new ArrayVsListComparison();
            
            // Convert to array for comparison
            Issue[] issueArray = issues.ToArray();
            
            // Performance testing
            var stopwatch = Stopwatch.StartNew();
            
            // Array operations
            stopwatch.Restart();
            for (int i = 0; i < issueArray.Length; i++)
            {
                var issue = issueArray[i]; // Direct indexing
            }
            comparison.ArrayAccessTime = stopwatch.ElapsedMilliseconds;
            
            // List operations
            stopwatch.Restart();
            for (int i = 0; i < issues.Count; i++)
            {
                var issue = issues[i]; // Indexer access
            }
            comparison.ListAccessTime = stopwatch.ElapsedMilliseconds;
            
            // Memory usage comparison
            comparison.ArrayMemoryUsage = GC.GetTotalMemory(false);
            
            // Array advantages
            comparison.ArrayAdvantages = new List<string>
            {
                "Fixed size - memory efficient",
                "Direct memory access - faster indexing",
                "Better for mathematical operations",
                "Lower memory overhead"
            };
            
            // List advantages
            comparison.ListAdvantages = new List<string>
            {
                "Dynamic size - grows as needed",
                "Built-in methods (Add, Remove, etc.)",
                "Better for frequent insertions/deletions",
                "LINQ support out of the box"
            };
            
            // Recommendations
            comparison.Recommendations = new List<string>
            {
                "Use Arrays for: Fixed-size data, performance-critical operations, mathematical computations",
                "Use Lists for: Dynamic collections, frequent modifications, LINQ operations"
            };
            
            return comparison;
        }

        #endregion

        #region Learning Unit 2: Advanced Generics Implementation

        /// <summary>
        /// Advanced generic repository with constraints
        /// Demonstrates generic type constraints and advanced C# features
        /// </summary>
        public class Repository<T> where T : class, new()
        {
            private readonly List<T> _items = new List<T>();
            private readonly Dictionary<Guid, T> _index = new Dictionary<Guid, T>();

            public void Add(T item)
            {
                if (item is Issue issue)
                {
                    _index[issue.Id] = item;
                }
                _items.Add(item);
            }

            public T? GetById(Guid id)
            {
                return _index.TryGetValue(id, out T? item) ? item : null;
            }

            public List<T> GetAll()
            {
                return new List<T>(_items);
            }

            public List<T> Find(Predicate<T> predicate)
            {
                return _items.FindAll(predicate);
            }

            public void Remove(T item)
            {
                if (item is Issue issue)
                {
                    _index.Remove(issue.Id);
                }
                _items.Remove(item);
            }

            public int Count => _items.Count;
        }

        /// <summary>
        /// Generic sorted collection with constraints
        /// Demonstrates IComparable constraint and custom sorting
        /// </summary>
        public class SortedCollection<T> where T : IComparable<T>
        {
            private readonly List<T> _items = new List<T>();

            public void Add(T item)
            {
                _items.Add(item);
                _items.Sort(); // Uses IComparable implementation
            }

            public T? GetMin()
            {
                return _items.Count > 0 ? _items[0] : default(T);
            }

            public T? GetMax()
            {
                return _items.Count > 0 ? _items[_items.Count - 1] : default(T);
            }

            public List<T> GetAll()
            {
                return new List<T>(_items);
            }
        }

        #endregion

        #region Learning Unit 2: Operator Overloading

        /// <summary>
        /// Helper methods for Issue operations
        /// Demonstrates custom operations for complex objects
        /// </summary>
        public static class IssueOperations
        {
            public static bool IsGreaterThan(Issue? issue1, Issue? issue2)
            {
                if (issue1 == null) return false;
                if (issue2 == null) return true;
                return (issue1.Priority > issue2.Priority) || 
                       ((issue1.Priority == issue2.Priority) && (issue1.UserPoints > issue2.UserPoints));
            }

            public static bool IsLessThan(Issue? issue1, Issue? issue2)
            {
                if (issue1 == null) return issue2 != null;
                if (issue2 == null) return false;
                return issue1.Priority < issue2.Priority || 
                       (issue1.Priority == issue2.Priority && issue1.UserPoints < issue2.UserPoints);
            }

            public static bool IsGreaterThanOrEqual(Issue? issue1, Issue? issue2)
            {
                if (issue1 == null) return issue2 == null;
                if (issue2 == null) return true;
                return IsGreaterThan(issue1, issue2) || issue1.Equals(issue2);
            }

            public static bool IsLessThanOrEqual(Issue? issue1, Issue? issue2)
            {
                if (issue1 == null) return true;
                if (issue2 == null) return false;
                return IsLessThan(issue1, issue2) || issue1.Equals(issue2);
            }

            public static Issue MergeIssues(Issue? issue1, Issue? issue2)
            {
                if (issue1 == null || issue2 == null)
                    throw new ArgumentNullException("Cannot merge null issues");
                
                // Merge two issues into one (for demonstration)
                return new Issue
                {
                    Title = $"{issue1.Title} + {issue2.Title}",
                    Description = $"{issue1.Description}\n\nMerged with: {issue2.Description}",
                    Category = issue1.Category,
                    Priority = (issue1.Priority > issue2.Priority) ? issue1.Priority : issue2.Priority,
                    Location = issue1.Location,
                    UserPoints = issue1.UserPoints + issue2.UserPoints,
                    UserEmail = issue1.UserEmail
                };
            }

            public static Issue ScaleIssue(Issue? issue, int multiplier)
            {
                if (issue == null)
                    throw new ArgumentNullException("Cannot scale null issue");
                
                // Scale issue points (for demonstration)
                return new Issue
                {
                    Title = issue.Title,
                    Description = issue.Description,
                    Category = issue.Category,
                    Priority = issue.Priority,
                    Location = issue.Location,
                    UserPoints = issue.UserPoints * multiplier,
                    UserEmail = issue.UserEmail
                };
            }
        }

        #endregion

        #region Learning Unit 2: Recursion Implementation

        /// <summary>
        /// Recursive method to count issues by category
        /// Demonstrates recursion for data processing
        /// </summary>
        public static int CountIssuesRecursively(List<Issue> issues, IssueCategory category, int index = 0)
        {
            // Base case
            if (index >= issues.Count)
                return 0;
            
            // Recursive case
            int count = issues[index].Category == category ? 1 : 0;
            return count + CountIssuesRecursively(issues, category, index + 1);
        }

        /// <summary>
        /// Recursive method to filter issues by condition
        /// Demonstrates recursion with predicates
        /// </summary>
        public static List<Issue> FilterIssuesRecursive(List<Issue> issues, Predicate<Issue> condition, int index = 0)
        {
            // Base case
            if (index >= issues.Count)
                return new List<Issue>();
            
            // Recursive case
            var result = FilterIssuesRecursive(issues, condition, index + 1);
            
            if (condition(issues[index]))
            {
                result.Insert(0, issues[index]); // Insert at beginning to maintain order
            }
            
            return result;
        }

        /// <summary>
        /// Recursive method to calculate total points
        /// Demonstrates recursion with accumulation
        /// </summary>
        public static int CalculateTotalPointsRecursively(List<Issue> issues, int index = 0)
        {
            // Base case
            if (index >= issues.Count)
                return 0;
            
            // Recursive case
            return issues[index].UserPoints + CalculateTotalPointsRecursively(issues, index + 1);
        }

        /// <summary>
        /// Recursive method to find issues by priority
        /// Demonstrates recursion with complex conditions
        /// </summary>
        public static List<Issue> FindIssuesByPriorityRecursively(List<Issue> issues, IssuePriority priority, int index = 0)
        {
            // Base case
            if (index >= issues.Count)
                return new List<Issue>();
            
            // Recursive case
            var result = FindIssuesByPriorityRecursively(issues, priority, index + 1);
            
            if (issues[index].Priority == priority)
            {
                result.Insert(0, issues[index]);
            }
            
            return result;
        }

        /// <summary>
        /// Recursive method to filter issues by condition
        /// Demonstrates recursion with predicates
        /// </summary>
        public static List<Issue> FilterIssuesRecursively(List<Issue> issues, Predicate<Issue> condition, int index = 0)
        {
            // Base case
            if (index >= issues.Count)
                return new List<Issue>();
            
            // Recursive case
            var result = FilterIssuesRecursively(issues, condition, index + 1);
            
            if (condition(issues[index]))
            {
                result.Insert(0, issues[index]); // Insert at beginning to maintain order
            }
            
            return result;
        }

        #endregion
    }

    #region Supporting Classes

    public class GeographicalStats
    {
        public List<CategoryZoneCount> CategoryZoneCounts { get; set; } = new List<CategoryZoneCount>();
        public List<PriorityCategoryZoneCount> PriorityCategoryZoneCounts { get; set; } = new List<PriorityCategoryZoneCount>();
    }

    public class CategoryZoneCount
    {
        public IssueCategory Category { get; set; }
        public int Zone { get; set; }
        public int Count { get; set; }
    }

    public class PriorityCategoryZoneCount
    {
        public IssuePriority Priority { get; set; }
        public IssueCategory Category { get; set; }
        public int Zone { get; set; }
        public int Count { get; set; }
    }

    public class ArrayVsListComparison
    {
        public long ArrayAccessTime { get; set; }
        public long ListAccessTime { get; set; }
        public long ArrayMemoryUsage { get; set; }
        public List<string> ArrayAdvantages { get; set; } = new List<string>();
        public List<string> ListAdvantages { get; set; } = new List<string>();
        public List<string> Recommendations { get; set; } = new List<string>();
    }

    #endregion
}
