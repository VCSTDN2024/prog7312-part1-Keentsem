using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.Services;
using MunicipalServicesApp.Data;
using System.Diagnostics;

namespace MunicipalServicesApp.Pages
{
    public class AdvancedFeaturesModel : PageModel
    {
        private readonly AdvancedDataStructuresService _advancedService;
        private readonly IssuesRepository _issuesRepository;
        private readonly PerformanceAnalyzer _performanceAnalyzer;

        public AdvancedFeaturesModel(AdvancedDataStructuresService advancedService, IssuesRepository issuesRepository, PerformanceAnalyzer performanceAnalyzer)
        {
            _advancedService = advancedService;
            _issuesRepository = issuesRepository;
            _performanceAnalyzer = performanceAnalyzer;
        }

        public void OnGet()
        {
            // Initialize the page
            ViewData["TotalPoints"] = _issuesRepository.GetTotalUserPoints();
        }

        #region Service Prioritization Tools

        public IActionResult OnGetQuickSort()
        {
            var issues = _issuesRepository.GetAllIssues().Take(10).ToArray();
            if (issues.Length == 0)
            {
                return Content("<p class='alert alert-warning'>No issues available for sorting. Please add some issues first.</p>", "text/html");
            }

            var originalIssues = issues.Select(i => new { i.Title, i.Priority, i.UserPoints }).ToList();

            var stopwatch = Stopwatch.StartNew();
            AdvancedDataStructuresService.QuickSortIssues(issues, 0, issues.Length - 1);
            stopwatch.Stop();

            var sortedIssues = issues.Select(i => new { i.Title, i.Priority, i.UserPoints }).ToList();

            var result = $@"
                <p><strong>Service Urgency Sorting Performance:</strong> {stopwatch.ElapsedMilliseconds}ms</p>
                <p><strong>Original Service Order:</strong></p>
                <ul>{string.Join("", originalIssues.Select(i => $"<li>{i.Title} - Urgency: {i.Priority}, Impact: {i.UserPoints}</li>"))}</ul>
                <p><strong>Sorted by Urgency (most urgent first):</strong></p>
                <ul>{string.Join("", sortedIssues.Select(i => $"<li>{i.Title} - Urgency: {i.Priority}, Impact: {i.UserPoints}</li>"))}</ul>
                <p><em>This helps municipal staff prioritize the most urgent community issues first.</em></p>
            ";

            return Content(result, "text/html");
        }

        public IActionResult OnGetMergeSort()
        {
            var issues = _issuesRepository.GetAllIssues().Take(10).ToList();
            if (issues.Count == 0)
            {
                return Content("<p class='alert alert-warning'>No issues available for sorting. Please add some issues first.</p>", "text/html");
            }

            var originalIssues = issues.Select(i => new { i.Title, i.SubmittedAt, i.UserPoints }).ToList();

            var stopwatch = Stopwatch.StartNew();
            AdvancedDataStructuresService.MergeSortByDate(issues);
            stopwatch.Stop();

            var sortedIssues = issues.Select(i => new { i.Title, i.SubmittedAt, i.UserPoints }).ToList();

            var result = $@"
                <p><strong>Service Timeline Sorting Performance:</strong> {stopwatch.ElapsedMilliseconds}ms</p>
                <p><strong>Original Service Order:</strong></p>
                <ul>{string.Join("", originalIssues.Select(i => $"<li>{i.Title} - Reported: {i.SubmittedAt:yyyy-MM-dd}, Impact: {i.UserPoints}</li>"))}</ul>
                <p><strong>Sorted by Date (most recent first):</strong></p>
                <ul>{string.Join("", sortedIssues.Select(i => $"<li>{i.Title} - Reported: {i.SubmittedAt:yyyy-MM-dd}, Impact: {i.UserPoints}</li>"))}</ul>
                <p><em>This helps municipal staff process community issues in chronological order.</em></p>
            ";

            return Content(result, "text/html");
        }

        public IActionResult OnGetHeapSort()
        {
            var issues = _issuesRepository.GetAllIssues().Take(10).ToArray();
            if (issues.Length == 0)
            {
                return Content("<p class='alert alert-warning'>No issues available for sorting. Please add some issues first.</p>", "text/html");
            }

            var originalIssues = issues.Select(i => new { i.Title, i.UserPoints, i.Priority }).ToList();

            var stopwatch = Stopwatch.StartNew();
            AdvancedDataStructuresService.HeapSortByPoints(issues);
            stopwatch.Stop();

            var sortedIssues = issues.Select(i => new { i.Title, i.UserPoints, i.Priority }).ToList();

            var result = $@"
                <p><strong>Community Impact Sorting Performance:</strong> {stopwatch.ElapsedMilliseconds}ms</p>
                <p><strong>Original Service Order:</strong></p>
                <ul>{string.Join("", originalIssues.Select(i => $"<li>{i.Title} - Impact: {i.UserPoints}, Urgency: {i.Priority}</li>"))}</ul>
                <p><strong>Sorted by Impact (highest community impact first):</strong></p>
                <ul>{string.Join("", sortedIssues.Select(i => $"<li>{i.Title} - Impact: {i.UserPoints}, Urgency: {i.Priority}</li>"))}</ul>
                <p><em>This helps municipal staff prioritize issues with the highest community impact.</em></p>
            ";

            return Content(result, "text/html");
        }

        public IActionResult OnGetCompareArrayVsList()
        {
            var issues = _issuesRepository.GetAllIssues().Take(1000).ToList();
            var comparison = _performanceAnalyzer.CompareDataStructures(issues);

            var result = $@"
                <div class='row'>
                    <div class='col-md-6'>
                        <h6>Service Efficiency Comparison:</h6>
                        <ul>
                            <li><strong>Array Sequential Access:</strong> {comparison.ArraySequentialAccessTime}ms</li>
                            <li><strong>List Sequential Access:</strong> {comparison.ListSequentialAccessTime}ms</li>
                            <li><strong>Array Random Access:</strong> {comparison.ArrayRandomAccessTime}ms</li>
                            <li><strong>List Random Access:</strong> {comparison.ListRandomAccessTime}ms</li>
                            <li><strong>Array Memory Usage:</strong> {comparison.ArrayMemoryUsage:N0} bytes</li>
                            <li><strong>List Memory Usage:</strong> {comparison.ListMemoryUsage:N0} bytes</li>
                        </ul>
                    </div>
                    <div class='col-md-6'>
                        <h6>Performance Analysis:</h6>
                        <ul>
                            <li><strong>Array Insertion Time:</strong> {comparison.ArrayInsertionTime}ms</li>
                            <li><strong>List Insertion Time:</strong> {comparison.ListInsertionTime}ms</li>
                            <li><strong>Array Search Time:</strong> {comparison.ArraySearchTime}ms</li>
                            <li><strong>List Search Time:</strong> {comparison.ListSearchTime}ms</li>
                        </ul>
                    </div>
                </div>
                <div class='mt-3'>
                    <h6>Municipal Service Recommendations:</h6>
                    <ul>{string.Join("", comparison.Recommendations.Select(r => $"<li>{r}</li>"))}</ul>
                </div>
                <p><em>This analysis helps optimize municipal data management for better service delivery.</em></p>
            ";

            return Content(result, "text/html");
        }

        public IActionResult OnGetAnalyzeAlgorithmComplexity()
        {
            var complexity = _performanceAnalyzer.AnalyzeComplexity();

            var result = $@"
                <div class='row'>
                    <div class='col-md-6'>
                        <h6>QuickSort Analysis:</h6>
                        <ul>
                            <li><strong>Time Complexity:</strong> {complexity.QuickSort.TimeComplexity}</li>
                            <li><strong>Space Complexity:</strong> {complexity.QuickSort.SpaceComplexity}</li>
                            <li><strong>Best Case:</strong> {complexity.QuickSort.BestCase}</li>
                            <li><strong>Worst Case:</strong> {complexity.QuickSort.WorstCase}</li>
                            <li><strong>Stable:</strong> {(complexity.QuickSort.Stable ? "Yes" : "No")}</li>
                            <li><strong>In-Place:</strong> {(complexity.QuickSort.InPlace ? "Yes" : "No")}</li>
                            <li><strong>Use Case:</strong> {complexity.QuickSort.UseCase}</li>
                        </ul>
                    </div>
                    <div class='col-md-6'>
                        <h6>MergeSort Analysis:</h6>
                        <ul>
                            <li><strong>Time Complexity:</strong> {complexity.MergeSort.TimeComplexity}</li>
                            <li><strong>Space Complexity:</strong> {complexity.MergeSort.SpaceComplexity}</li>
                            <li><strong>Best Case:</strong> {complexity.MergeSort.BestCase}</li>
                            <li><strong>Worst Case:</strong> {complexity.MergeSort.WorstCase}</li>
                            <li><strong>Stable:</strong> {(complexity.MergeSort.Stable ? "Yes" : "No")}</li>
                            <li><strong>In-Place:</strong> {(complexity.MergeSort.InPlace ? "Yes" : "No")}</li>
                            <li><strong>Use Case:</strong> {complexity.MergeSort.UseCase}</li>
                        </ul>
                    </div>
                </div>
                <div class='row mt-3'>
                    <div class='col-md-6'>
                        <h6>HeapSort Analysis:</h6>
                        <ul>
                            <li><strong>Time Complexity:</strong> {complexity.HeapSort.TimeComplexity}</li>
                            <li><strong>Space Complexity:</strong> {complexity.HeapSort.SpaceComplexity}</li>
                            <li><strong>Best Case:</strong> {complexity.HeapSort.BestCase}</li>
                            <li><strong>Worst Case:</strong> {complexity.HeapSort.WorstCase}</li>
                            <li><strong>Stable:</strong> {(complexity.HeapSort.Stable ? "Yes" : "No")}</li>
                            <li><strong>In-Place:</strong> {(complexity.HeapSort.InPlace ? "Yes" : "No")}</li>
                            <li><strong>Use Case:</strong> {complexity.HeapSort.UseCase}</li>
                        </ul>
                    </div>
                    <div class='col-md-6'>
                        <h6>InsertionSort Analysis:</h6>
                        <ul>
                            <li><strong>Time Complexity:</strong> {complexity.InsertionSort.TimeComplexity}</li>
                            <li><strong>Space Complexity:</strong> {complexity.InsertionSort.SpaceComplexity}</li>
                            <li><strong>Best Case:</strong> {complexity.InsertionSort.BestCase}</li>
                            <li><strong>Worst Case:</strong> {complexity.InsertionSort.WorstCase}</li>
                            <li><strong>Stable:</strong> {(complexity.InsertionSort.Stable ? "Yes" : "No")}</li>
                            <li><strong>In-Place:</strong> {(complexity.InsertionSort.InPlace ? "Yes" : "No")}</li>
                            <li><strong>Use Case:</strong> {complexity.InsertionSort.UseCase}</li>
                        </ul>
                    </div>
                </div>
                <p><em>This analysis helps municipal staff choose the right sorting algorithm for different service scenarios.</em></p>
            ";

            return Content(result, "text/html");
        }

        #endregion

        #region Community Issue Management

        public IActionResult OnGetTestOperators()
        {
            var issues = _issuesRepository.GetAllIssues().Take(3).ToList();
            if (issues.Count < 2) 
            {
                return Content("<p class='alert alert-warning'>Not enough community issues to compare. Please add more issues first.</p>", "text/html");
            }

            var issue1 = issues[0];
            var issue2 = issues[1];

            var result = $@"
                <p><strong>Community Issue Comparison:</strong></p>
                <p><strong>Issue 1:</strong> {issue1.Title} (Urgency: {issue1.Priority}, Impact: {issue1.UserPoints})</p>
                <p><strong>Issue 2:</strong> {issue2.Title} (Urgency: {issue2.Priority}, Impact: {issue2.UserPoints})</p>
                <ul>
                    <li><strong>Issue 1 has higher priority than Issue 2:</strong> {AdvancedDataStructuresService.IssueOperations.IsGreaterThan(issue1, issue2)}</li>
                    <li><strong>Issue 1 has lower priority than Issue 2:</strong> {AdvancedDataStructuresService.IssueOperations.IsLessThan(issue1, issue2)}</li>
                    <li><strong>Issue 1 priority >= Issue 2:</strong> {AdvancedDataStructuresService.IssueOperations.IsGreaterThanOrEqual(issue1, issue2)}</li>
                    <li><strong>Issue 1 priority <= Issue 2:</strong> {AdvancedDataStructuresService.IssueOperations.IsLessThanOrEqual(issue1, issue2)}</li>
                    <li><strong>Issues are identical:</strong> {issue1 == issue2}</li>
                    <li><strong>Issues are different:</strong> {issue1 != issue2}</li>
                </ul>
                <p><em>This helps municipal staff quickly compare and prioritize community issues.</em></p>
            ";

            return Content(result, "text/html");
        }

        public IActionResult OnGetMergeIssues()
        {
            var issues = _issuesRepository.GetAllIssues().Take(2).ToList();
            if (issues.Count < 2) 
            {
                return Content("<p class='alert alert-warning'>Not enough community issues to merge. Please add more issues first.</p>", "text/html");
            }

            var issue1 = issues[0];
            var issue2 = issues[1];
            var mergedIssue = AdvancedDataStructuresService.IssueOperations.MergeIssues(issue1, issue2); // Using merge method

            var result = $@"
                <p><strong>Community Issue Consolidation:</strong></p>
                <p><strong>Original Issue 1:</strong> {issue1.Title} (Impact: {issue1.UserPoints})</p>
                <p><strong>Original Issue 2:</strong> {issue2.Title} (Impact: {issue2.UserPoints})</p>
                <hr>
                <p><strong>Consolidated Community Issue:</strong></p>
                <ul>
                    <li><strong>Title:</strong> {mergedIssue.Title}</li>
                    <li><strong>Description:</strong> {mergedIssue.Description.Substring(0, Math.Min(100, mergedIssue.Description.Length))}...</li>
                    <li><strong>Urgency:</strong> {mergedIssue.Priority}</li>
                    <li><strong>Total Impact:</strong> {mergedIssue.UserPoints} (combined community impact)</li>
                    <li><strong>Attachments:</strong> {mergedIssue.AttachmentPaths.Count} files</li>
                </ul>
                <p><em>This helps municipal staff consolidate related community issues for more efficient service delivery.</em></p>
            ";

            return Content(result, "text/html");
        }

        public IActionResult OnGetScaleIssue()
        {
            var issues = _issuesRepository.GetAllIssues().Take(1).ToList();
            if (!issues.Any()) 
            {
                return Content("<p class='alert alert-warning'>No community issues to analyze. Please add some issues first.</p>", "text/html");
            }

            var issue = issues[0];
            var scaledIssue = AdvancedDataStructuresService.IssueOperations.ScaleIssue(issue, 2); // Using scale method

            var result = $@"
                <p><strong>Community Impact Analysis:</strong></p>
                <p><strong>Original Issue:</strong> {issue.Title} (Impact: {issue.UserPoints})</p>
                <hr>
                <p><strong>Scaled Community Impact (× 2 multiplier):</strong></p>
                <ul>
                    <li><strong>Title:</strong> {scaledIssue.Title}</li>
                    <li><strong>Community Impact:</strong> {scaledIssue.UserPoints} (original: {issue.UserPoints} × 2)</li>
                    <li><strong>Urgency:</strong> {scaledIssue.Priority}</li>
                    <li><strong>Service Category:</strong> {scaledIssue.Category}</li>
                </ul>
                <p><em>This helps municipal staff understand the potential community impact of scaling up service responses.</em></p>
            ";

            return Content(result, "text/html");
        }

        #endregion

        #region Community Analytics

        public IActionResult OnGetCountRecursively()
        {
            var issues = _issuesRepository.GetAllIssues().ToList();
            if (!issues.Any())
            {
                return Content("<p class='alert alert-warning'>No community issues to analyze. Please add some issues first.</p>", "text/html");
            }

            var categoryCounts = new Dictionary<IssueCategory, int>();

            foreach (IssueCategory category in Enum.GetValues<IssueCategory>())
            {
                var count = AdvancedDataStructuresService.CountIssuesRecursively(issues, category);
                categoryCounts[category] = count;
            }

            var result = $@"
                <p><strong>Community Service Analysis by Category:</strong></p>
                <ul>{string.Join("", categoryCounts.Select(kvp => $"<li><strong>{kvp.Key}:</strong> {kvp.Value} community issues</li>"))}</ul>
                <p><em>This provides deep insights into community service needs across different categories.</em></p>
            ";

            return Content(result, "text/html");
        }

        public IActionResult OnGetFilterRecursively()
        {
            var issues = _issuesRepository.GetAllIssues().ToList();
            if (!issues.Any())
            {
                return Content("<p class='alert alert-warning'>No community issues to analyze. Please add some issues first.</p>", "text/html");
            }

            var criticalIssues = AdvancedDataStructuresService.FilterIssuesRecursively(issues, i => i.Priority == IssuePriority.Critical);

            var moreText = criticalIssues.Count > 5 ? "<p><em>... and more urgent issues</em></p>" : "";
            var result = $@"
                <p><strong>Community Urgent Issues Analysis:</strong></p>
                <p><strong>Total Community Issues:</strong> {issues.Count}</p>
                <p><strong>Urgent Issues Requiring Immediate Attention:</strong> {criticalIssues.Count}</p>
                <ul>{string.Join("", criticalIssues.Take(5).Select(i => $"<li>{i.Title} - Urgency: {i.Priority}, Impact: {i.UserPoints}</li>"))}</ul>
                {moreText}
                <p><em>This helps municipal staff identify the most urgent community issues that need immediate attention.</em></p>
            ";

            return Content(result, "text/html");
        }

        public IActionResult OnGetCalculatePointsRecursively()
        {
            var issues = _issuesRepository.GetAllIssues().ToList();
            if (!issues.Any())
            {
                return Content("<p class='alert alert-warning'>No community issues to analyze. Please add some issues first.</p>", "text/html");
            }

            var totalPoints = AdvancedDataStructuresService.CalculateTotalPointsRecursively(issues);

            var result = $@"
                <p><strong>Community Impact Score Calculation:</strong></p>
                <p><strong>Total Community Issues:</strong> {issues.Count}</p>
                <p><strong>Total Community Impact Score:</strong> {totalPoints}</p>
                <p><strong>Average Impact per Issue:</strong> {(double)totalPoints / issues.Count:F2}</p>
                <p><em>This provides a comprehensive measure of community engagement and service impact.</em></p>
            ";

            return Content(result, "text/html");
        }

        #endregion

        #region Data Transparency Tools

        public IActionResult OnGetTestGenerics()
        {
            var issues = _issuesRepository.GetAllIssues().Take(5).ToList();
            
            // Test generic repository with constraints
            var repository = new GenericRepository<Issue>();
            foreach (var issue in issues)
            {
                repository.Add(issue);
            }

            var foundIssue = repository.GetById(issues.First().Id);
            var allIssues = repository.GetAll();
            var criticalIssues = repository.Find(i => i.Priority == IssuePriority.Critical);
            var stats = repository.GetStatistics();

            // Test pagination
            var paginatedResult = repository.GetPaginated(1, 3);

            var result = $@"
                <p><strong>Municipal Data Repository Test:</strong></p>
                <ul>
                    <li><strong>Repository Type:</strong> Generic Repository with Type Constraints</li>
                    <li><strong>Community Issues Added:</strong> {issues.Count}</li>
                    <li><strong>Total in Repository:</strong> {repository.Count}</li>
                    <li><strong>Issue Found by ID:</strong> {((foundIssue != null) ? "Yes" : "No")}</li>
                    <li><strong>All Issues Retrieved:</strong> {allIssues.Count}</li>
                    <li><strong>Urgent Issues Found:</strong> {criticalIssues.Count}</li>
                    <li><strong>Indexed Items:</strong> {stats.IndexedItems}</li>
                    <li><strong>Memory Usage:</strong> {stats.MemoryUsage:N0} bytes</li>
                    <li><strong>Pagination Test:</strong> Page 1 of {paginatedResult.TotalPages} (showing {paginatedResult.Items.Count} items)</li>
                </ul>
                <p><em>This demonstrates advanced generic repository with type constraints for municipal operations.</em></p>
            ";

            return Content(result, "text/html");
        }

        #endregion

        #region Community Events & Public Life Integration

        public IActionResult OnGetAnalyzeGeographicalData()
        {
            var issues = _issuesRepository.GetAllIssues().ToList();
            if (!issues.Any())
            {
                return Content("<p class='alert alert-warning'>No community issues to analyze. Please add some issues first.</p>", "text/html");
            }

            _advancedService.PopulateGeographicalData(issues);
            var stats = _advancedService.GetGeographicalStats();

            var result = $@"
                <p><strong>Community Events & Public Life Integration Analysis:</strong></p>
                <div class='row'>
                    <div class='col-md-6'>
                        <h6>Service Categories by Community Zone:</h6>
                        <ul>{string.Join("", stats.CategoryZoneCounts.Take(10).Select(c => $"<li>{c.Category} in Zone {c.Zone}: {c.Count} community needs</li>"))}</ul>
                    </div>
                    <div class='col-md-6'>
                        <h6>Service Priority by Category & Zone:</h6>
                        <ul>{string.Join("", stats.PriorityCategoryZoneCounts.Take(10).Select(p => $"<li>{p.Priority} {p.Category} in Zone {p.Zone}: {p.Count} urgent needs</li>"))}</ul>
                    </div>
                </div>
                <p><em>This helps connect municipal services with community events and public life activities across different zones.</em></p>
            ";

            return Content(result, "text/html");
        }

        #endregion
    }
}
