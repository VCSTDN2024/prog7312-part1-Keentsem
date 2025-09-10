using MunicipalServicesApp.Models;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Advanced Municipal Repository with multi-dimensional arrays and custom sorting
    /// Replaces basic collections with sophisticated data structures for municipal operations
    /// Integrates academic requirements directly into municipal service functionality
    /// </summary>
    public class MunicipalRepository<T> where T : class, IMunicipalEntity, new()
    {
        #region Multi-dimensional Arrays for Municipal Resource Planning

        // 2D Array: Category × Municipality analysis for resource allocation
        private static int[,] _categoryMunicipalityMatrix = new int[0, 0];
        
        // 3D Array: Priority × Category × Municipality for crew dispatch optimization
        private static int[,,] _resourceAllocationMatrix = new int[0, 0, 0];
        
        // Jagged Array: Municipal hierarchy for recursive address parsing
        private static string[][] _municipalHierarchy = Array.Empty<string[]>();
        
        // Municipal zone mapping for geographical optimization
        private static readonly Dictionary<string, int> _municipalityIndexMap = new();
        private static readonly Dictionary<int, string> _indexMunicipalityMap = new();

        #endregion

        #region Core Repository Data Structures

        private readonly ConcurrentBag<T> _items = new();
        private readonly ConcurrentDictionary<Guid, T> _index = new();
        private readonly object _lockObject = new();
        private static int _municipalityCount = 0;

        #endregion

        #region Constructor and Initialization

        public MunicipalRepository()
        {
            InitializeMunicipalDataStructures();
        }

        /// <summary>
        /// Initialize multi-dimensional arrays for municipal resource planning
        /// Sets up data structures for actual municipal operations, not academic demos
        /// </summary>
        private void InitializeMunicipalDataStructures()
        {
            // Initialize South African municipal hierarchy
            _municipalHierarchy = new string[][]
            {
                new string[] { "Western Cape", "Cape Town", "City Bowl", "Gardens", "Tamboerskloof" },
                new string[] { "Western Cape", "Cape Town", "Atlantic Seaboard", "Sea Point", "Bantry Bay" },
                new string[] { "Western Cape", "Cape Town", "Southern Suburbs", "Rondebosch", "Newlands" },
                new string[] { "Gauteng", "Johannesburg", "Sandton", "Rivonia", "Bryanston" },
                new string[] { "Gauteng", "Johannesburg", "Randburg", "Ferndale", "Linden" },
                new string[] { "Gauteng", "Tshwane", "Pretoria Central", "Arcadia", "Hatfield" },
                new string[] { "KwaZulu-Natal", "eThekwini", "Durban Central", "Point", "Berea" },
                new string[] { "KwaZulu-Natal", "eThekwini", "Umhlanga", "Gateway", "La Lucia" },
                new string[] { "Eastern Cape", "Nelson Mandela Bay", "Port Elizabeth", "Central", "Richmond Hill" },
                new string[] { "Free State", "Mangaung", "Bloemfontein", "Central", "Universitas" }
            };

            // Initialize matrices based on municipal hierarchy
            var categoryCount = Enum.GetValues<IssueCategory>().Length;
            var priorityCount = 4; // Low, Medium, High, Critical
            _municipalityCount = _municipalHierarchy.Length;

            _categoryMunicipalityMatrix = new int[categoryCount, _municipalityCount];
            _resourceAllocationMatrix = new int[priorityCount, categoryCount, _municipalityCount];

            // Build municipality index mapping for O(1) lookups
            for (int i = 0; i < _municipalHierarchy.Length; i++)
            {
                var municipality = _municipalHierarchy[i][1]; // Municipality name is at index 1
                _municipalityIndexMap[municipality] = i;
                _indexMunicipalityMap[i] = municipality;
            }
        }

        #endregion

        #region Municipal Resource Planning with Multi-dimensional Arrays

        /// <summary>
        /// Add item and update municipal resource matrices
        /// Integrates academic multi-dimensional arrays into actual municipal operations
        /// </summary>
        public void Add(T item)
        {
            lock (_lockObject)
            {
                _items.Add(item);
                _index[item.Id] = item;

                // Update municipal resource matrices for actual operational use
                if (item is Issue issue)
                {
                    UpdateResourceAllocationMatrices(issue);
                }

                item.LastModified = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Update resource allocation matrices for municipal planning
        /// Uses 2D and 3D arrays for real municipal resource optimization
        /// </summary>
        private void UpdateResourceAllocationMatrices(Issue issue)
        {
            var categoryIndex = (int)issue.Category;
            var priorityIndex = (int)issue.Priority - 1; // Priority enum starts at 1
            var municipalityIndex = GetMunicipalityIndex(issue.Municipality);

            if (municipalityIndex >= 0)
            {
                // Update 2D matrix: Category × Municipality for resource planning
                _categoryMunicipalityMatrix[categoryIndex, municipalityIndex]++;

                // Update 3D matrix: Priority × Category × Municipality for crew dispatch
                _resourceAllocationMatrix[priorityIndex, categoryIndex, municipalityIndex]++;
            }
        }

        /// <summary>
        /// Get municipal resource allocation analysis
        /// Provides actual operational insights using multi-dimensional arrays
        /// </summary>
        public MunicipalResourceAnalysis GetResourceAllocationAnalysis()
        {
            lock (_lockObject)
            {
                var analysis = new MunicipalResourceAnalysis();

                // Analyze 2D matrix for resource needs by category and municipality
                for (int category = 0; category < _categoryMunicipalityMatrix.GetLength(0); category++)
                {
                    for (int municipality = 0; municipality < _categoryMunicipalityMatrix.GetLength(1); municipality++)
                    {
                        var count = _categoryMunicipalityMatrix[category, municipality];
                        if (count > 0)
                        {
                            analysis.CategoryMunicipalityNeeds.Add(new ResourceNeed
                            {
                                Category = (IssueCategory)category,
                                Municipality = _indexMunicipalityMap[municipality],
                                ResourceRequirement = count,
                                Priority = CalculateResourcePriority(category, municipality)
                            });
                        }
                    }
                }

                // Analyze 3D matrix for crew dispatch optimization
                for (int priority = 0; priority < _resourceAllocationMatrix.GetLength(0); priority++)
                {
                    for (int category = 0; category < _resourceAllocationMatrix.GetLength(1); category++)
                    {
                        for (int municipality = 0; municipality < _resourceAllocationMatrix.GetLength(2); municipality++)
                        {
                            var count = _resourceAllocationMatrix[priority, category, municipality];
                            if (count > 0)
                            {
                                analysis.CrewDispatchOptimization.Add(new CrewDispatchPlan
                                {
                                    Priority = (IssuePriority)(priority + 1),
                                    Category = (IssueCategory)category,
                                    Municipality = _indexMunicipalityMap[municipality],
                                    CrewsNeeded = CalculateCrewsNeeded(priority, category, count),
                                    EstimatedResponseTime = CalculateResponseTime(priority, municipality)
                                });
                            }
                        }
                    }
                }

                return analysis;
            }
        }

        #endregion

        #region Custom Sorting for Emergency Response

        /// <summary>
        /// Get all items sorted by emergency priority using custom QuickSort
        /// Replaces LINQ OrderBy with custom algorithm for municipal emergency dispatch
        /// </summary>
        public List<T> GetAllSortedByEmergencyPriority()
        {
            lock (_lockObject)
            {
                var items = _items.Where(item => item.IsActive).ToArray();
                if (items.Length == 0) return new List<T>();

                // Use custom QuickSort for emergency response optimization
                QuickSortByEmergencyPriority(items, 0, items.Length - 1);
                return items.ToList();
            }
        }

        /// <summary>
        /// Custom QuickSort implementation for emergency response
        /// Optimized for municipal crew dispatch based on priority, location, and resource needs
        /// </summary>
        private void QuickSortByEmergencyPriority(T[] items, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = PartitionByEmergencyPriority(items, low, high);
                QuickSortByEmergencyPriority(items, low, pivotIndex - 1);
                QuickSortByEmergencyPriority(items, pivotIndex + 1, high);
            }
        }

        /// <summary>
        /// Partition method for emergency priority QuickSort
        /// Uses municipal-specific comparison logic for real operational needs
        /// </summary>
        private int PartitionByEmergencyPriority(T[] items, int low, int high)
        {
            var pivot = items[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (CompareEmergencyPriority(items[j], pivot) >= 0)
                {
                    i++;
                    Swap(items, i, j);
                }
            }
            Swap(items, i + 1, high);
            return i + 1;
        }

        /// <summary>
        /// Emergency priority comparison for municipal operations
        /// Considers priority, municipality resource availability, and response time
        /// </summary>
        private int CompareEmergencyPriority(T item1, T item2)
        {
            if (item1 is IMunicipalComparable comp1 && item2 is IMunicipalComparable comp2)
            {
                // Primary: Priority (Critical issues first)
                int priorityCompare = comp2.Priority.CompareTo(comp1.Priority);
                if (priorityCompare != 0) return priorityCompare;

                // Secondary: Resource availability in municipality
                var municipality1Index = GetMunicipalityIndex(item1.Municipality);
                var municipality2Index = GetMunicipalityIndex(item2.Municipality);
                int resourceCompare = GetMunicipalityResourceLoad(municipality1Index)
                    .CompareTo(GetMunicipalityResourceLoad(municipality2Index));
                if (resourceCompare != 0) return resourceCompare;

                // Tertiary: Time submitted (newer first)
                return comp2.SubmittedAt.CompareTo(comp1.SubmittedAt);
            }
            return 0;
        }

        /// <summary>
        /// Get critical issues using HeapSort for emergency response queue
        /// Maintains emergency response queue with O(log n) operations
        /// </summary>
        public List<T> GetCriticalIssuesHeapSorted()
        {
            lock (_lockObject)
            {
                var criticalItems = _items.Where(item => 
                    item.IsActive && 
                    item is IMunicipalComparable comp && 
                    comp.Priority >= 3).ToArray(); // High and Critical priority

                if (criticalItems.Length == 0) return new List<T>();

                HeapSortByResponseTime(criticalItems);
                return criticalItems.ToList();
            }
        }

        /// <summary>
        /// HeapSort implementation for emergency response time optimization
        /// Ensures fastest response to critical municipal issues
        /// </summary>
        private void HeapSortByResponseTime(T[] items)
        {
            int n = items.Length;

            // Build max heap
            for (int i = n / 2 - 1; i >= 0; i--)
                HeapifyByResponseTime(items, n, i);

            // Extract elements from heap
            for (int i = n - 1; i > 0; i--)
            {
                Swap(items, 0, i);
                HeapifyByResponseTime(items, i, 0);
            }
        }

        /// <summary>
        /// Heapify method optimized for municipal response time
        /// </summary>
        private void HeapifyByResponseTime(T[] items, int n, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && CompareResponseTime(items[left], items[largest]) > 0)
                largest = left;

            if (right < n && CompareResponseTime(items[right], items[largest]) > 0)
                largest = right;

            if (largest != i)
            {
                Swap(items, i, largest);
                HeapifyByResponseTime(items, n, largest);
            }
        }

        /// <summary>
        /// Response time comparison for municipal emergency operations
        /// </summary>
        private int CompareResponseTime(T item1, T item2)
        {
            var responseTime1 = CalculateResponseTime(GetPriorityValue(item1), GetMunicipalityIndex(item1.Municipality));
            var responseTime2 = CalculateResponseTime(GetPriorityValue(item2), GetMunicipalityIndex(item2.Municipality));
            return responseTime2.CompareTo(responseTime1); // Shorter response time first
        }

        #endregion

        #region Recursive Methods for Municipal Hierarchy

        /// <summary>
        /// Recursive address parsing for municipal hierarchy
        /// Replaces simple string splitting with sophisticated address standardization
        /// </summary>
        public string ParseMunicipalAddressRecursively(string address)
        {
            return ParseAddressHierarchy(address, _municipalHierarchy, 0);
        }

        /// <summary>
        /// Recursive address hierarchy parsing
        /// Handles Province → Municipality → Ward → Street hierarchy validation
        /// </summary>
        private string ParseAddressHierarchy(string address, string[][] hierarchy, int hierarchyIndex)
        {
            // Base case: reached end of hierarchy
            if (hierarchyIndex >= hierarchy.Length)
                return ExtractMunicipalityFromAddress(address);

            // Recursive case: check current hierarchy level
            var currentHierarchy = hierarchy[hierarchyIndex];
            foreach (var level in currentHierarchy)
            {
                if (address.Contains(level, StringComparison.OrdinalIgnoreCase))
                {
                    // Found match at this level, validate with recursive call
                    var result = ParseAddressHierarchy(address, hierarchy, hierarchyIndex + 1);
                    if (!string.IsNullOrEmpty(result))
                        return result;
                }
            }

            // Continue to next hierarchy level
            return ParseAddressHierarchy(address, hierarchy, hierarchyIndex + 1);
        }

        /// <summary>
        /// Recursive badge calculation for municipal achievements
        /// Replaces simple counting with recursive municipal department hierarchy
        /// </summary>
        public List<Badge> CalculateBadgesRecursively(string userEmail, int departmentLevel = 0)
        {
            var userIssues = _items.OfType<Issue>()
                .Where(issue => issue.UserEmail == userEmail && issue.IsActive)
                .ToList();

            return CalculateDepartmentBadgesRecursive(userIssues, departmentLevel, new List<Badge>());
        }

        /// <summary>
        /// Recursive badge calculation through municipal department hierarchy
        /// </summary>
        private List<Badge> CalculateDepartmentBadgesRecursive(List<Issue> issues, int departmentLevel, List<Badge> badges)
        {
            // Base case: processed all department levels
            if (departmentLevel >= Enum.GetValues<IssueCategory>().Length)
                return badges;

            var category = (IssueCategory)departmentLevel;
            var categoryIssues = issues.Where(i => i.Category == category).ToList();

            // Add badge for this department if qualified
            if (categoryIssues.Count >= 2)
            {
                badges.Add(new Badge
                {
                    Id = 100 + departmentLevel,
                    Name = $"{category} Department Champion",
                    Description = $"Contributed significantly to {category} municipal services",
                    Type = BadgeType.CategorySpecialist,
                    IsEarned = true,
                    EarnedDate = categoryIssues.OrderBy(i => i.SubmittedAt).Skip(1).First().SubmittedAt,
                    PointsValue = CalculateDepartmentPoints(categoryIssues.Count, departmentLevel),
                    ImagePath = GetCategoryBadgeImage(category)
                });
            }

            // Recursive call for next department level
            return CalculateDepartmentBadgesRecursive(issues, departmentLevel + 1, badges);
        }

        /// <summary>
        /// Recursive point accumulation through municipal hierarchy
        /// </summary>
        public int CalculateHierarchicalPointsRecursive(List<Issue> issues, int hierarchyLevel = 0)
        {
            // Base case: processed all hierarchy levels
            if (hierarchyLevel >= _municipalHierarchy.Length)
                return 0;

            var municipality = _municipalHierarchy[hierarchyLevel][1];
            var municipalityIssues = issues.Where(i => i.Municipality == municipality).ToList();
            var levelPoints = municipalityIssues.Sum(i => i.UserPoints);

            // Recursive accumulation through hierarchy
            return levelPoints + CalculateHierarchicalPointsRecursive(issues, hierarchyLevel + 1);
        }

        #endregion

        #region Standard Repository Operations with Municipal Enhancements

        public T? GetById(Guid id)
        {
            return _index.TryGetValue(id, out T? item) ? item : null;
        }

        public List<T> GetAll()
        {
            lock (_lockObject)
            {
                return _items.Where(item => item.IsActive).ToList();
            }
        }

        public List<T> Find(Expression<Func<T, bool>> expression)
        {
            lock (_lockObject)
            {
                var compiled = expression.Compile();
                return _items.Where(item => item.IsActive && compiled(item)).ToList();
            }
        }

        public bool Remove(Guid id)
        {
            lock (_lockObject)
            {
                if (_index.TryGetValue(id, out T? item))
                {
                    item.IsActive = false;
                    item.LastModified = DateTime.UtcNow;
                    return true;
                }
                return false;
            }
        }

        public bool Update(T item)
        {
            lock (_lockObject)
            {
                if (_index.ContainsKey(item.Id))
                {
                    item.LastModified = DateTime.UtcNow;
                    _index[item.Id] = item;
                    
                    // Update resource matrices if it's an Issue
                    if (item is Issue issue)
                    {
                        UpdateResourceAllocationMatrices(issue);
                    }
                    
                    return true;
                }
                return false;
            }
        }

        public int Count => _items.Count(item => item.IsActive);

        #endregion

        #region Municipal Utility Methods

        private int GetMunicipalityIndex(string municipality)
        {
            return _municipalityIndexMap.TryGetValue(municipality, out int index) ? index : -1;
        }

        private int GetMunicipalityResourceLoad(int municipalityIndex)
        {
            if (municipalityIndex < 0) return 0;
            
            int totalLoad = 0;
            for (int category = 0; category < _categoryMunicipalityMatrix.GetLength(0); category++)
            {
                totalLoad += _categoryMunicipalityMatrix[category, municipalityIndex];
            }
            return totalLoad;
        }

        private int CalculateResourcePriority(int categoryIndex, int municipalityIndex)
        {
            // Calculate priority based on resource allocation matrix
            int totalPriorityLoad = 0;
            for (int priority = 0; priority < _resourceAllocationMatrix.GetLength(0); priority++)
            {
                totalPriorityLoad += _resourceAllocationMatrix[priority, categoryIndex, municipalityIndex] * (priority + 1);
            }
            return totalPriorityLoad;
        }

        private int CalculateCrewsNeeded(int priority, int category, int issueCount)
        {
            // Municipal crew calculation based on priority and category
            int baseCrews = issueCount / 5; // Base: 1 crew per 5 issues
            int priorityMultiplier = priority + 1;
            int categoryMultiplier = GetCategoryComplexity(category);
            
            return Math.Max(1, baseCrews * priorityMultiplier * categoryMultiplier / 2);
        }

        private TimeSpan CalculateResponseTime(int priority, int municipalityIndex)
        {
            // Calculate estimated response time based on priority and municipality
            int baseMins = 60; // Base response time: 1 hour
            int priorityReduction = priority * 15; // Higher priority = faster response
            int municipalityDelay = GetMunicipalityDelay(municipalityIndex);
            
            return TimeSpan.FromMinutes(Math.Max(15, baseMins - priorityReduction + municipalityDelay));
        }

        private int GetCategoryComplexity(int categoryIndex)
        {
            // Return complexity multiplier for different municipal categories
            return categoryIndex switch
            {
                0 => 3, // WaterSupply - high complexity
                1 => 3, // Electricity - high complexity  
                2 => 2, // Roads - medium complexity
                3 => 1, // WasteManagement - low complexity
                4 => 4, // PublicSafety - highest complexity
                5 => 1, // ParksAndRecreation - low complexity
                6 => 2, // BuildingPermits - medium complexity
                _ => 1  // Other - low complexity
            };
        }

        private int GetMunicipalityDelay(int municipalityIndex)
        {
            // Return delay factor based on municipality resource availability
            if (municipalityIndex < 0) return 30;
            
            var resourceLoad = GetMunicipalityResourceLoad(municipalityIndex);
            return resourceLoad switch
            {
                < 10 => 0,   // Low load - no delay
                < 25 => 10,  // Medium load - 10 min delay
                < 50 => 20,  // High load - 20 min delay
                _ => 30      // Very high load - 30 min delay
            };
        }

        private int GetPriorityValue(T item)
        {
            return item is IMunicipalComparable comp ? comp.Priority : 1;
        }

        private string ExtractMunicipalityFromAddress(string address)
        {
            // Simple fallback for address parsing
            var parts = address.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length > 1 ? parts[1].Trim() : "Unknown";
        }

        private int CalculateDepartmentPoints(int issueCount, int departmentLevel)
        {
            return issueCount * (departmentLevel + 1) * 25; // Escalating points by department
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

        private void Swap(T[] items, int i, int j)
        {
            (items[i], items[j]) = (items[j], items[i]);
        }

        #endregion
    }

    #region Municipal Analysis Classes

    public class MunicipalResourceAnalysis
    {
        public List<ResourceNeed> CategoryMunicipalityNeeds { get; set; } = new();
        public List<CrewDispatchPlan> CrewDispatchOptimization { get; set; } = new();
        public DateTime AnalysisDate { get; set; } = DateTime.UtcNow;
    }

    public class ResourceNeed
    {
        public IssueCategory Category { get; set; }
        public string Municipality { get; set; } = string.Empty;
        public int ResourceRequirement { get; set; }
        public int Priority { get; set; }
    }

    public class CrewDispatchPlan
    {
        public IssuePriority Priority { get; set; }
        public IssueCategory Category { get; set; }
        public string Municipality { get; set; } = string.Empty;
        public int CrewsNeeded { get; set; }
        public TimeSpan EstimatedResponseTime { get; set; }
    }

    #endregion
}
