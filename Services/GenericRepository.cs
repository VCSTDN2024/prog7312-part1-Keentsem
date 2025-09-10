using MunicipalServicesApp.Models;
using System.Linq.Expressions;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Advanced Generic Repository with Type Constraints for Learning Unit 2
    /// Demonstrates generic classes with constraints and advanced C# features
    /// </summary>
    /// <typeparam name="T">Entity type that must be a class, comparable, and have a parameterless constructor</typeparam>
    public class GenericRepository<T> where T : class, IComparable<T>, new()
    {
        private readonly List<T> _items = new List<T>();
        private readonly Dictionary<Guid, T> _index = new Dictionary<Guid, T>();
        private readonly object _lockObject = new object();

        /// <summary>
        /// Add an item to the repository
        /// Thread-safe operation with locking
        /// </summary>
        public void Add(T item)
        {
            lock (_lockObject)
            {
                if (item is Issue issue)
                {
                    _index[issue.Id] = item;
                }
                _items.Add(item);
            }
        }

        /// <summary>
        /// Add multiple items in a batch operation
        /// Demonstrates batch processing for performance
        /// </summary>
        public void AddRange(IEnumerable<T> items)
        {
            lock (_lockObject)
            {
                foreach (var item in items)
                {
                    if (item is Issue issue)
                    {
                        _index[issue.Id] = item;
                    }
                    _items.Add(item);
                }
            }
        }

        /// <summary>
        /// Get an item by ID (for entities with Guid Id property)
        /// </summary>
        public T? GetById(Guid id)
        {
            lock (_lockObject)
            {
                return _index.TryGetValue(id, out T? item) ? item : null;
            }
        }

        /// <summary>
        /// Get all items from the repository
        /// Returns a copy to prevent external modification
        /// </summary>
        public List<T> GetAll()
        {
            lock (_lockObject)
            {
                return new List<T>(_items);
            }
        }

        /// <summary>
        /// Find items using a predicate
        /// Demonstrates LINQ-like functionality
        /// </summary>
        public List<T> FindAll(Predicate<T> predicate)
        {
            lock (_lockObject)
            {
                return _items.FindAll(predicate);
            }
        }

        /// <summary>
        /// Find items using an expression tree
        /// Demonstrates advanced querying capabilities
        /// </summary>
        public List<T> Find(Expression<Func<T, bool>> expression)
        {
            lock (_lockObject)
            {
                var compiledExpression = expression.Compile();
                return _items.Where(compiledExpression).ToList();
            }
        }

        /// <summary>
        /// Remove an item from the repository
        /// </summary>
        public bool Remove(T item)
        {
            lock (_lockObject)
            {
                if (item is Issue issue)
                {
                    _index.Remove(issue.Id);
                }
                return _items.Remove(item);
            }
        }

        /// <summary>
        /// Remove an item by ID
        /// </summary>
        public bool RemoveById(Guid id)
        {
            lock (_lockObject)
            {
                if (_index.TryGetValue(id, out T? item))
                {
                    _index.Remove(id);
                    return _items.Remove(item);
                }
                return false;
            }
        }

        /// <summary>
        /// Update an existing item
        /// </summary>
        public bool Update(T item)
        {
            lock (_lockObject)
            {
                if (item is Issue issue)
                {
                    if (_index.TryGetValue(issue.Id, out T? existingItem))
                    {
                        var index = _items.IndexOf(existingItem);
                        if (index >= 0)
                        {
                            _items[index] = item;
                            _index[issue.Id] = item;
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Get the count of items in the repository
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lockObject)
                {
                    return _items.Count;
                }
            }
        }

        /// <summary>
        /// Check if the repository contains an item
        /// </summary>
        public bool Contains(T item)
        {
            lock (_lockObject)
            {
                return _items.Contains(item);
            }
        }

        /// <summary>
        /// Clear all items from the repository
        /// </summary>
        public void Clear()
        {
            lock (_lockObject)
            {
                _items.Clear();
                _index.Clear();
            }
        }

        /// <summary>
        /// Get items with pagination support
        /// Demonstrates pagination for large datasets
        /// </summary>
        public PaginatedResult<T> GetPaginated(int pageNumber, int pageSize)
        {
            lock (_lockObject)
            {
                var totalCount = _items.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                var items = _items
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return new PaginatedResult<T>
                {
                    Items = items,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    HasPreviousPage = pageNumber > 1,
                    HasNextPage = pageNumber < totalPages
                };
            }
        }

        /// <summary>
        /// Get items sorted by a key selector
        /// Demonstrates generic sorting with IComparable constraint
        /// </summary>
        public List<T> GetSorted<TKey>(Func<T, TKey> keySelector, bool ascending = true)
        {
            lock (_lockObject)
            {
                return ascending
                    ? _items.OrderBy(keySelector).ToList()
                    : _items.OrderByDescending(keySelector).ToList();
            }
        }

        /// <summary>
        /// Get statistics about the repository
        /// Demonstrates comprehensive data analysis
        /// </summary>
        public RepositoryStatistics GetStatistics()
        {
            lock (_lockObject)
            {
                return new RepositoryStatistics
                {
                    TotalItems = _items.Count,
                    IndexedItems = _index.Count,
                    MemoryUsage = GC.GetTotalMemory(false),
                    LastUpdated = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Batch process items in chunks
        /// Demonstrates memory-conscious batch processing
        /// </summary>
        public void ProcessInBatches(int batchSize, Action<List<T>> batchProcessor)
        {
            lock (_lockObject)
            {
                for (int i = 0; i < _items.Count; i += batchSize)
                {
                    var batch = _items.Skip(i).Take(batchSize).ToList();
                    batchProcessor(batch);
                }
            }
        }

        /// <summary>
        /// Get items by multiple criteria
        /// Demonstrates complex querying
        /// </summary>
        public List<T> FindByMultipleCriteria(List<Expression<Func<T, bool>>> criteria)
        {
            lock (_lockObject)
            {
                var query = _items.AsQueryable();
                
                foreach (var criterion in criteria)
                {
                    query = query.Where(criterion);
                }
                
                return query.ToList();
            }
        }
    }

    /// <summary>
    /// Paginated result for repository queries
    /// </summary>
    public class PaginatedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
    }

    /// <summary>
    /// Repository statistics
    /// </summary>
    public class RepositoryStatistics
    {
        public int TotalItems { get; set; }
        public int IndexedItems { get; set; }
        public long MemoryUsage { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Generic sorted collection with constraints
    /// Demonstrates IComparable constraint and custom sorting
    /// </summary>
    public class SortedCollection<T> where T : IComparable<T>
    {
        private readonly List<T> _items = new List<T>();

        /// <summary>
        /// Add an item to the sorted collection
        /// Automatically maintains sorted order
        /// </summary>
        public void Add(T item)
        {
            _items.Add(item);
            _items.Sort(); // Uses IComparable implementation
        }

        /// <summary>
        /// Get the minimum item
        /// </summary>
        public T? GetMin()
        {
            return _items.Count > 0 ? _items[0] : default(T);
        }

        /// <summary>
        /// Get the maximum item
        /// </summary>
        public T? GetMax()
        {
            return _items.Count > 0 ? _items[_items.Count - 1] : default(T);
        }

        /// <summary>
        /// Get all items in sorted order
        /// </summary>
        public List<T> GetAll()
        {
            return new List<T>(_items);
        }

        /// <summary>
        /// Get items in a range
        /// </summary>
        public List<T> GetRange(T min, T max)
        {
            return _items.Where(item => item.CompareTo(min) >= 0 && item.CompareTo(max) <= 0).ToList();
        }

        /// <summary>
        /// Get the count of items
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Check if the collection contains an item
        /// </summary>
        public bool Contains(T item)
        {
            return _items.Contains(item);
        }

        /// <summary>
        /// Remove an item
        /// </summary>
        public bool Remove(T item)
        {
            return _items.Remove(item);
        }

        /// <summary>
        /// Clear all items
        /// </summary>
        public void Clear()
        {
            _items.Clear();
        }
    }
}