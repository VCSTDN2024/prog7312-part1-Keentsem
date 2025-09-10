using MunicipalServicesApp.Models;
using MunicipalServicesApp.Data;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Service for managing users using multi-dimensional arrays
    /// Demonstrates advanced data structure implementation for municipal user management
    /// </summary>
    public class MultiDimensionalUserService
    {
        // Multi-dimensional array for user data storage
        // Dimensions: [UserIndex, DataField, TimeIndex]
        // DataField indices: 0=ID, 1=Username, 2=Email, 3=Points, 4=Level, 5=Reports, 6=Badges, 7=LastActive
        private static object[,,] _userDataArray = new object[1000, 8, 10]; // 1000 users, 8 data fields, 10 time snapshots
        private static int _currentUserCount = 0;
        private static int _currentTimeIndex = 0;
        
        // Jagged array for user sessions (more flexible than multi-dimensional)
        private static object[][] _userSessions = new object[1000][];
        
        // 3D array for user activity tracking
        // Dimensions: [UserIndex, ActivityType, TimeSlot]
        private static int[,,] _userActivityMatrix = new int[1000, 10, 24]; // 1000 users, 10 activity types, 24 hours
        
        private readonly UserRepository _userRepository;

        public MultiDimensionalUserService(UserRepository userRepository)
        {
            _userRepository = userRepository;
            InitializeDemoUsers();
        }

        /// <summary>
        /// Initialize demo users in multi-dimensional arrays
        /// </summary>
        private void InitializeDemoUsers()
        {
            // Demo User 1
            AddUserToArray(0, new User
            {
                Id = 1,
                Username = "demo",
                Email = "demo@municipal.gov.za",
                TotalPoints = 150,
                Level = UserLevel.Silver,
                Municipality = "Cape Town",
                Province = "Western Cape",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastActiveAt = DateTime.UtcNow
            });

            // Demo User 2
            AddUserToArray(1, new User
            {
                Id = 2,
                Username = "admin",
                Email = "admin@municipal.gov.za",
                TotalPoints = 500,
                Level = UserLevel.Platinum,
                Municipality = "Johannesburg",
                Province = "Gauteng",
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                LastActiveAt = DateTime.UtcNow
            });

            // Demo User 3
            AddUserToArray(2, new User
            {
                Id = 3,
                Username = "citizen",
                Email = "citizen@example.com",
                TotalPoints = 75,
                Level = UserLevel.Bronze,
                Municipality = "Durban",
                Province = "KwaZulu-Natal",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                LastActiveAt = DateTime.UtcNow.AddDays(-1)
            });

            _currentUserCount = 3;
        }

        /// <summary>
        /// Add user to multi-dimensional array
        /// </summary>
        private void AddUserToArray(int userIndex, User user)
        {
            _userDataArray[userIndex, 0, _currentTimeIndex] = user.Id; // ID
            _userDataArray[userIndex, 1, _currentTimeIndex] = user.Username; // Username
            _userDataArray[userIndex, 2, _currentTimeIndex] = user.Email; // Email
            _userDataArray[userIndex, 3, _currentTimeIndex] = user.TotalPoints; // Points
            _userDataArray[userIndex, 4, _currentTimeIndex] = (int)user.Level; // Level
            _userDataArray[userIndex, 5, _currentTimeIndex] = 0; // Reports count
            _userDataArray[userIndex, 6, _currentTimeIndex] = 0; // Badges count
            _userDataArray[userIndex, 7, _currentTimeIndex] = user.LastActiveAt; // Last Active

            // Initialize user session jagged array
            _userSessions[userIndex] = new object[10];
            _userSessions[userIndex][0] = user.Id;
            _userSessions[userIndex][1] = user.Username;
            _userSessions[userIndex][2] = user.Email;
            _userSessions[userIndex][3] = user.TotalPoints;
            _userSessions[userIndex][4] = (int)user.Level;
            _userSessions[userIndex][5] = user.Municipality;
            _userSessions[userIndex][6] = user.Province;
            _userSessions[userIndex][7] = user.CreatedAt;
            _userSessions[userIndex][8] = user.LastActiveAt;
            _userSessions[userIndex][9] = user.IsActive;
        }

        /// <summary>
        /// Get user by index from multi-dimensional array
        /// </summary>
        public User? GetUserByIndex(int userIndex)
        {
            if (userIndex >= _currentUserCount) return null;

            try
            {
                return new User
                {
                    Id = (int)_userDataArray[userIndex, 0, _currentTimeIndex],
                    Username = _userDataArray[userIndex, 1, _currentTimeIndex]?.ToString() ?? "",
                    Email = _userDataArray[userIndex, 2, _currentTimeIndex]?.ToString() ?? "",
                    TotalPoints = (int)_userDataArray[userIndex, 3, _currentTimeIndex],
                    Level = (UserLevel)(int)_userDataArray[userIndex, 4, _currentTimeIndex],
                    LastActiveAt = (DateTime)_userDataArray[userIndex, 7, _currentTimeIndex],
                    IsActive = true
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Update user points in multi-dimensional array
        /// </summary>
        public void UpdateUserPoints(int userIndex, int newPoints)
        {
            if (userIndex >= _currentUserCount) return;

            _userDataArray[userIndex, 3, _currentTimeIndex] = newPoints;
            
            // Update level based on points
            var newLevel = CalculateUserLevel(newPoints);
            _userDataArray[userIndex, 4, _currentTimeIndex] = (int)newLevel;
            
            // Update last active time
            _userDataArray[userIndex, 7, _currentTimeIndex] = DateTime.UtcNow;
        }

        /// <summary>
        /// Get all users from multi-dimensional array
        /// </summary>
        public List<User> GetAllUsersFromArray()
        {
            var users = new List<User>();
            
            for (int i = 0; i < _currentUserCount; i++)
            {
                var user = GetUserByIndex(i);
                if (user != null)
                {
                    users.Add(user);
                }
            }
            
            return users;
        }

        /// <summary>
        /// Track user activity in 3D matrix
        /// </summary>
        public void TrackUserActivity(int userIndex, ActivityType activityType, int hourOfDay)
        {
            if (userIndex >= _currentUserCount || hourOfDay < 0 || hourOfDay >= 24) return;

            _userActivityMatrix[userIndex, (int)activityType, hourOfDay]++;
        }

        /// <summary>
        /// Get user activity statistics from 3D matrix
        /// </summary>
        public UserActivityStats GetUserActivityStats(int userIndex)
        {
            if (userIndex >= _currentUserCount) return new UserActivityStats();

            var stats = new UserActivityStats();
            
            // Calculate total activities per type
            for (int activityType = 0; activityType < 10; activityType++)
            {
                int totalForType = 0;
                for (int hour = 0; hour < 24; hour++)
                {
                    totalForType += _userActivityMatrix[userIndex, activityType, hour];
                }
                stats.ActivityCounts[activityType] = totalForType;
            }

            // Find peak activity hour
            int maxHour = 0;
            int maxActivities = 0;
            for (int hour = 0; hour < 24; hour++)
            {
                int hourTotal = 0;
                for (int activityType = 0; activityType < 10; activityType++)
                {
                    hourTotal += _userActivityMatrix[userIndex, activityType, hour];
                }
                if (hourTotal > maxActivities)
                {
                    maxActivities = hourTotal;
                    maxHour = hour;
                }
            }
            stats.PeakActivityHour = maxHour;

            return stats;
        }

        /// <summary>
        /// Get user ranking using multi-dimensional array operations
        /// </summary>
        public List<UserRanking> GetUserRankings()
        {
            var rankings = new List<UserRanking>();
            
            for (int i = 0; i < _currentUserCount; i++)
            {
                var user = GetUserByIndex(i);
                if (user != null)
                {
                    rankings.Add(new UserRanking
                    {
                        UserIndex = i,
                        Username = user.Username,
                        Points = user.TotalPoints,
                        Level = user.Level,
                        Rank = 0 // Will be calculated below
                    });
                }
            }

            // Sort by points (descending) and assign ranks
            rankings = rankings.OrderByDescending(r => r.Points).ToList();
            for (int i = 0; i < rankings.Count; i++)
            {
                rankings[i].Rank = i + 1;
            }

            return rankings;
        }

        /// <summary>
        /// Create time snapshot of user data
        /// </summary>
        public void CreateTimeSnapshot()
        {
            _currentTimeIndex = (_currentTimeIndex + 1) % 10; // Cycle through 10 time slots
            
            // Copy current data to new time slot
            for (int userIndex = 0; userIndex < _currentUserCount; userIndex++)
            {
                for (int field = 0; field < 8; field++)
                {
                    _userDataArray[userIndex, field, _currentTimeIndex] = 
                        _userDataArray[userIndex, field, (_currentTimeIndex + 9) % 10];
                }
            }
        }

        /// <summary>
        /// Get user data history from multi-dimensional array
        /// </summary>
        public List<UserDataHistory> GetUserDataHistory(int userIndex)
        {
            if (userIndex >= _currentUserCount) return new List<UserDataHistory>();

            var history = new List<UserDataHistory>();
            
            for (int timeSlot = 0; timeSlot < 10; timeSlot++)
            {
                try
                {
                    var data = new UserDataHistory
                    {
                        TimeSlot = timeSlot,
                        UserId = (int)_userDataArray[userIndex, 0, timeSlot],
                        Username = _userDataArray[userIndex, 1, timeSlot]?.ToString() ?? "",
                        Points = (int)_userDataArray[userIndex, 3, timeSlot],
                        Level = (UserLevel)(int)_userDataArray[userIndex, 4, timeSlot],
                        ReportsCount = (int)_userDataArray[userIndex, 5, timeSlot],
                        BadgesCount = (int)_userDataArray[userIndex, 6, timeSlot],
                        LastActive = (DateTime)_userDataArray[userIndex, 7, timeSlot]
                    };
                    history.Add(data);
                }
                catch
                {
                    // Skip invalid time slots
                }
            }

            return history;
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
    }

    /// <summary>
    /// Activity types for tracking user behavior
    /// </summary>
    public enum ActivityType
    {
        ReportSubmitted = 0,
        BadgeEarned = 1,
        Login = 2,
        Logout = 3,
        ProfileView = 4,
        NotificationRead = 5,
        IssueViewed = 6,
        SearchPerformed = 7,
        SettingsChanged = 8,
        CommunityInteraction = 9
    }

    /// <summary>
    /// User activity statistics from 3D matrix
    /// </summary>
    public class UserActivityStats
    {
        public int[] ActivityCounts { get; set; } = new int[10];
        public int PeakActivityHour { get; set; }
        public int TotalActivities => ActivityCounts.Sum();
    }

    /// <summary>
    /// User ranking information
    /// </summary>
    public class UserRanking
    {
        public int UserIndex { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Points { get; set; }
        public UserLevel Level { get; set; }
        public int Rank { get; set; }
    }

    /// <summary>
    /// User data history from multi-dimensional array
    /// </summary>
    public class UserDataHistory
    {
        public int TimeSlot { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Points { get; set; }
        public UserLevel Level { get; set; }
        public int ReportsCount { get; set; }
        public int BadgesCount { get; set; }
        public DateTime LastActive { get; set; }
    }
}
