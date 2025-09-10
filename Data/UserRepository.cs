using System;
using System.Collections;
using System.Linq;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// Repository class for managing user data and gamification tracking.
    /// Uses ArrayList for dynamic, non-generic storage as requested for portfolio demonstration.
    /// Integrates with multi-dimensional arrays for advanced data structure demonstration.
    /// Provides methods for user registration, authentication, and badge management.
    /// </summary>
    public class UserRepository
    {
        /// <summary>
        /// Static ArrayList to store all registered users in memory
        /// Using ArrayList for maximum flexibility and demonstration purposes
        /// </summary>
        private static readonly ArrayList _users = new ArrayList();
        
        // ========================================
        // MULTI-DIMENSIONAL ARRAYS - User Data Management
        // ========================================
        // These arrays provide efficient storage and historical tracking of user data
        
        /// <summary>
        /// 3D Array: User Data Storage with Historical Tracking
        /// Dimensions: [UserIndex, DataField, TimeIndex]
        /// DataField indices: 0=ID, 1=Username, 2=Email, 3=Points, 4=Level, 5=Reports, 6=Badges, 7=LastActive
        /// Purpose: Store user data with time-series tracking for analytics
        /// Usage: User progression tracking, historical analysis, data mining
        /// Capacity: 1000 users, 8 data fields, 10 time snapshots
        /// </summary>
        private static object[,,] _userDataArray = new object[1000, 8, 10];
        private static int _currentUserCount = 0;
        private static int _currentTimeIndex = 0;
        
        /// <summary>
        /// 2D Array: New User Registration Tracking
        /// Dimensions: [UserIndex, RegistrationData]
        /// RegistrationData indices: 0=RegistrationDate, 1=Source, 2=InitialPoints, 3=Status
        /// Purpose: Track new user registrations and their initial data
        /// Usage: Registration analytics, user onboarding tracking, growth metrics
        /// Capacity: 1000 new user registrations
        /// </summary>
        private static object[,] _newUserRegistrations = new object[1000, 4];
        private static int _newUserCount = 0;

        /// <summary>
        /// Counter for generating unique user IDs
        /// </summary>
        private static int _nextUserId = 1;

        /// <summary>
        /// Static constructor to initialize demo users
        /// </summary>
        static UserRepository()
        {
            InitializeDemoUsers();
        }

        /// <summary>
        /// Initialize demo users for testing and demonstration
        /// </summary>
        private static void InitializeDemoUsers()
        {
            // Demo user 1
            var demoUser1 = new User
            {
                Id = _nextUserId++,
                Username = "demo",
                Password = "demo123",
                Email = "demo@municipal.gov.za",
                FullName = "Demo User",
                Municipality = "Cape Town",
                Province = "Western Cape",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                LastActiveAt = DateTime.UtcNow,
                IsActive = true,
                TotalPoints = 150,
                Level = UserLevel.Silver
            };
            _users.Add(demoUser1);
            AddUserToMultiDimensionalArray(0, demoUser1);

            // Demo user 2
            var demoUser2 = new User
            {
                Id = _nextUserId++,
                Username = "admin",
                Password = "admin123",
                Email = "admin@municipal.gov.za",
                FullName = "Administrator",
                Municipality = "Johannesburg",
                Province = "Gauteng",
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                LastActiveAt = DateTime.UtcNow,
                IsActive = true,
                TotalPoints = 500,
                Level = UserLevel.Platinum
            };
            _users.Add(demoUser2);
            AddUserToMultiDimensionalArray(1, demoUser2);

            // Demo user 3
            var demoUser3 = new User
            {
                Id = _nextUserId++,
                Username = "citizen",
                Password = "citizen123",
                Email = "citizen@example.com",
                FullName = "Community Citizen",
                Municipality = "Durban",
                Province = "KwaZulu-Natal",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                LastActiveAt = DateTime.UtcNow.AddDays(-1),
                IsActive = true,
                TotalPoints = 75,
                Level = UserLevel.Bronze
            };
            _users.Add(demoUser3);
            AddUserToMultiDimensionalArray(2, demoUser3);

            _currentUserCount = 3;
        }

        /// <summary>
        /// Add user to multi-dimensional array for advanced data structure demonstration
        /// </summary>
        private static void AddUserToMultiDimensionalArray(int userIndex, User user)
        {
            _userDataArray[userIndex, 0, _currentTimeIndex] = user.Id; // ID
            _userDataArray[userIndex, 1, _currentTimeIndex] = user.Username; // Username
            _userDataArray[userIndex, 2, _currentTimeIndex] = user.Email; // Email
            _userDataArray[userIndex, 3, _currentTimeIndex] = user.TotalPoints; // Points
            _userDataArray[userIndex, 4, _currentTimeIndex] = (int)user.Level; // Level
            _userDataArray[userIndex, 5, _currentTimeIndex] = 0; // Reports count
            _userDataArray[userIndex, 6, _currentTimeIndex] = 0; // Badges count
            _userDataArray[userIndex, 7, _currentTimeIndex] = user.LastActiveAt; // Last Active
        }

        /// <summary>
        /// Update user points in multi-dimensional array
        /// </summary>
        public static void UpdateUserPointsInArray(int userId, int newPoints)
        {
            for (int i = 0; i < _currentUserCount; i++)
            {
                if ((int)_userDataArray[i, 0, _currentTimeIndex] == userId)
                {
                    _userDataArray[i, 3, _currentTimeIndex] = newPoints;
                    
                    // Update level based on points
                    var newLevel = CalculateUserLevel(newPoints);
                    _userDataArray[i, 4, _currentTimeIndex] = (int)newLevel;
                    
                    // Update last active time
                    _userDataArray[i, 7, _currentTimeIndex] = DateTime.UtcNow;
                    break;
                }
            }
        }

        /// <summary>
        /// Get user data from multi-dimensional array
        /// </summary>
        public static User? GetUserFromArray(int userId)
        {
            for (int i = 0; i < _currentUserCount; i++)
            {
                if ((int)_userDataArray[i, 0, _currentTimeIndex] == userId)
                {
                    try
                    {
                        return new User
                        {
                            Id = (int)_userDataArray[i, 0, _currentTimeIndex],
                            Username = _userDataArray[i, 1, _currentTimeIndex]?.ToString() ?? "",
                            Email = _userDataArray[i, 2, _currentTimeIndex]?.ToString() ?? "",
                            TotalPoints = (int)_userDataArray[i, 3, _currentTimeIndex],
                            Level = (UserLevel)(int)_userDataArray[i, 4, _currentTimeIndex],
                            LastActiveAt = (DateTime)_userDataArray[i, 7, _currentTimeIndex],
                            IsActive = true
                        };
                    }
                    catch
                    {
                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get all users from multi-dimensional array
        /// </summary>
        public static List<User> GetAllUsersFromArray()
        {
            var users = new List<User>();
            
            for (int i = 0; i < _currentUserCount; i++)
            {
                try
                {
                    var user = new User
                    {
                        Id = (int)_userDataArray[i, 0, _currentTimeIndex],
                        Username = _userDataArray[i, 1, _currentTimeIndex]?.ToString() ?? "",
                        Email = _userDataArray[i, 2, _currentTimeIndex]?.ToString() ?? "",
                        TotalPoints = (int)_userDataArray[i, 3, _currentTimeIndex],
                        Level = (UserLevel)(int)_userDataArray[i, 4, _currentTimeIndex],
                        LastActiveAt = (DateTime)_userDataArray[i, 7, _currentTimeIndex],
                        IsActive = true
                    };
                    users.Add(user);
                }
                catch
                {
                    // Skip invalid entries
                }
            }
            
            return users;
        }

        /// <summary>
        /// Create time snapshot of user data in multi-dimensional array
        /// </summary>
        public static void CreateTimeSnapshot()
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

        private static UserLevel CalculateUserLevel(int totalPoints)
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

        /// <summary>
        /// Gets all registered users
        /// </summary>
        /// <returns>ArrayList containing all users</returns>
        public ArrayList GetAllUsers()
        {
            return new ArrayList(_users);
        }

        /// <summary>
        /// Gets a user by their unique ID
        /// </summary>
        /// <param name="id">The user ID to search for</param>
        /// <returns>The User object if found, null otherwise</returns>
        public User? GetUserById(int id)
        {
            foreach (User user in _users)
            {
                if (user.Id == id)
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets a user by their username
        /// </summary>
        /// <param name="username">The username to search for</param>
        /// <returns>The User object if found, null otherwise</returns>
        public User? GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            foreach (User user in _users)
            {
                if (user.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Registers a new user in the system
        /// </summary>
        /// <param name="username">The username for the new user</param>
        /// <param name="password">The password for the new user</param>
        /// <param name="email">The email address for the new user (optional)</param>
        /// <param name="fullName">The full name for the new user (optional)</param>
        /// <param name="municipality">The municipality for the new user (optional)</param>
        /// <param name="province">The province for the new user (optional)</param>
        /// <returns>The newly created User object, or null if registration failed</returns>
        public User? RegisterUser(string username, string password, string email = "", string fullName = "", string municipality = "", string province = "")
        {
            // Validate input parameters
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            // Check if username already exists
            if (GetUserByUsername(username) != null)
            {
                return null; // Username already exists
            }

            // Create new user
            var newUser = new User
            {
                Id = _nextUserId++,
                Username = username,
                Password = password, // In production, this should be hashed
                Email = email,
                FullName = fullName,
                Municipality = municipality,
                Province = province,
                CreatedAt = DateTime.UtcNow,
                LastActiveAt = DateTime.UtcNow,
                IsActive = true
            };

            // Add to users collection
            _users.Add(newUser);
            return newUser;
        }

        /// <summary>
        /// Authenticates a user login attempt
        /// </summary>
        /// <param name="username">The username to authenticate</param>
        /// <param name="password">The password to verify</param>
        /// <returns>The User object if authentication successful, null otherwise</returns>
        public User? LoginUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            var user = GetUserByUsername(username);
            if (user != null && user.IsActive && user.Password == password)
            {
                user.UpdateLastActive();
                return user;
            }

            return null;
        }

        /// <summary>
        /// Assigns a badge to a user
        /// </summary>
        /// <param name="userId">The ID of the user to assign the badge to</param>
        /// <param name="badgeId">The ID or name of the badge to assign</param>
        /// <returns>True if badge was assigned successfully, false otherwise</returns>
        public bool AssignBadge(int userId, string badgeId)
        {
            var user = GetUserById(userId);
            if (user != null && !string.IsNullOrEmpty(badgeId))
            {
                user.AddBadge(badgeId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Assigns a badge to a user by username
        /// </summary>
        /// <param name="username">The username of the user to assign the badge to</param>
        /// <param name="badgeId">The ID or name of the badge to assign</param>
        /// <returns>True if badge was assigned successfully, false otherwise</returns>
        public bool AssignBadge(string username, string badgeId)
        {
            var user = GetUserByUsername(username);
            if (user != null && !string.IsNullOrEmpty(badgeId))
            {
                user.AddBadge(badgeId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets all badges earned by a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ArrayList of badge IDs/names, or empty ArrayList if user not found</returns>
        public ArrayList GetUserBadges(int userId)
        {
            var user = GetUserById(userId);
            return user?.Badges ?? new ArrayList();
        }

        /// <summary>
        /// Gets all badges earned by a specific user by username
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <returns>ArrayList of badge IDs/names, or empty ArrayList if user not found</returns>
        public ArrayList GetUserBadges(string username)
        {
            var user = GetUserByUsername(username);
            return user?.Badges ?? new ArrayList();
        }

        /// <summary>
        /// Gets all issues reported by a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>ArrayList of Issue objects, or empty ArrayList if user not found</returns>
        public ArrayList GetUserReportedIssues(int userId)
        {
            var user = GetUserById(userId);
            return user?.ReportedIssues ?? new ArrayList();
        }

        /// <summary>
        /// Gets all issues reported by a specific user by username
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <returns>ArrayList of Issue objects, or empty ArrayList if user not found</returns>
        public ArrayList GetUserReportedIssues(string username)
        {
            var user = GetUserByUsername(username);
            return user?.ReportedIssues ?? new ArrayList();
        }

        /// <summary>
        /// Adds a reported issue to a user's collection
        /// </summary>
        /// <param name="userId">The ID of the user who reported the issue</param>
        /// <param name="issue">The Issue object to add</param>
        /// <returns>True if issue was added successfully, false otherwise</returns>
        public bool AddUserReportedIssue(int userId, Issue issue)
        {
            var user = GetUserById(userId);
            if (user != null && issue != null)
            {
                user.AddReportedIssue(issue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a reported issue to a user's collection by username
        /// </summary>
        /// <param name="username">The username of the user who reported the issue</param>
        /// <param name="issue">The Issue object to add</param>
        /// <returns>True if issue was added successfully, false otherwise</returns>
        public bool AddUserReportedIssue(string username, Issue issue)
        {
            var user = GetUserByUsername(username);
            if (user != null && issue != null)
            {
                user.AddReportedIssue(issue);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the total number of registered users
        /// </summary>
        /// <returns>The count of users in the system</returns>
        public int GetUserCount()
        {
            return _users.Count;
        }

        /// <summary>
        /// Gets users by their level (for leaderboard purposes)
        /// </summary>
        /// <param name="level">The user level to filter by</param>
        /// <returns>ArrayList of users with the specified level</returns>
        public ArrayList GetUsersByLevel(UserLevel level)
        {
            var result = new ArrayList();
            foreach (User user in _users)
            {
                if (user.Level == level)
                {
                    result.Add(user);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the top users by total points (for leaderboard)
        /// </summary>
        /// <param name="count">The number of top users to return</param>
        /// <returns>ArrayList of users sorted by total points (descending)</returns>
        public ArrayList GetTopUsersByPoints(int count = 10)
        {
            var sortedUsers = new ArrayList();
            
            // Convert to array for sorting
            User[] userArray = new User[_users.Count];
            _users.CopyTo(userArray);
            
            // Sort by total points (descending)
            Array.Sort(userArray, (x, y) => y.TotalPoints.CompareTo(x.TotalPoints));
            
            // Take the top 'count' users
            for (int i = 0; i < Math.Min(count, userArray.Length); i++)
            {
                sortedUsers.Add(userArray[i]);
            }
            
            return sortedUsers;
        }

        /// <summary>
        /// Gets users by municipality (for regional statistics)
        /// </summary>
        /// <param name="municipality">The municipality to filter by</param>
        /// <returns>ArrayList of users from the specified municipality</returns>
        public ArrayList GetUsersByMunicipality(string municipality)
        {
            var result = new ArrayList();
            if (string.IsNullOrEmpty(municipality))
                return result;

            foreach (User user in _users)
            {
                if (user.Municipality?.Equals(municipality, StringComparison.OrdinalIgnoreCase) == true)
                {
                    result.Add(user);
                }
            }
            return result;
        }

        /// <summary>
        /// Updates a user's information
        /// </summary>
        /// <param name="userId">The ID of the user to update</param>
        /// <param name="email">New email address (optional)</param>
        /// <param name="fullName">New full name (optional)</param>
        /// <param name="municipality">New municipality (optional)</param>
        /// <param name="province">New province (optional)</param>
        /// <returns>True if user was updated successfully, false otherwise</returns>
        public bool UpdateUser(int userId, string email = "", string fullName = "", string municipality = "", string province = "")
        {
            var user = GetUserById(userId);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                if (!string.IsNullOrEmpty(fullName))
                    user.FullName = fullName;
                if (!string.IsNullOrEmpty(municipality))
                    user.Municipality = municipality;
                if (!string.IsNullOrEmpty(province))
                    user.Province = province;
                
                user.UpdateLastActive();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a user by email address
        /// </summary>
        /// <param name="email">The email address to search for</param>
        /// <returns>User object if found, null otherwise</returns>
        public User? GetUserByEmail(string email)
        {
            foreach (User user in _users)
            {
                if (user.Email.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates a user object (for point tracking and other updates)
        /// </summary>
        /// <param name="user">The user object to update</param>
        /// <returns>True if user was updated successfully, false otherwise</returns>
        public bool UpdateUser(User user)
        {
            var existingUser = GetUserById(user.Id);
            if (existingUser != null)
            {
                // Update the user in the ArrayList
                int index = _users.IndexOf(existingUser);
                if (index >= 0)
                {
                    _users[index] = user;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Registers a new user and adds to tracking arrays
        /// </summary>
        /// <param name="user">The user to register</param>
        /// <returns>True if registration was successful, false otherwise</returns>
        public bool RegisterNewUser(User user)
        {
            try
            {
                // Add to ArrayList
                _users.Add(user);
                
                // Add to multi-dimensional array
                AddUserToMultiDimensionalArray(_currentUserCount, user);
                
                // Add to new user registrations array
                _newUserRegistrations[_newUserCount, 0] = DateTime.UtcNow; // RegistrationDate
                _newUserRegistrations[_newUserCount, 1] = "Web Registration"; // Source
                _newUserRegistrations[_newUserCount, 2] = user.TotalPoints; // InitialPoints
                _newUserRegistrations[_newUserCount, 3] = "Active"; // Status
                
                _currentUserCount++;
                _newUserCount++;
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets all newly registered users from the registration array
        /// </summary>
        /// <returns>List of new user registration data</returns>
        public List<NewUserRegistration> GetNewUserRegistrations()
        {
            var registrations = new List<NewUserRegistration>();
            
            for (int i = 0; i < _newUserCount; i++)
            {
                try
                {
                    var registration = new NewUserRegistration
                    {
                        UserIndex = i,
                        RegistrationDate = (DateTime)_newUserRegistrations[i, 0],
                        Source = _newUserRegistrations[i, 1]?.ToString() ?? "",
                        InitialPoints = (int)_newUserRegistrations[i, 2],
                        Status = _newUserRegistrations[i, 3]?.ToString() ?? ""
                    };
                    registrations.Add(registration);
                }
                catch
                {
                    // Skip invalid entries
                }
            }
            
            return registrations;
        }

        /// <summary>
        /// Updates user reports count in multi-dimensional array
        /// </summary>
        /// <param name="userId">The user ID to update</param>
        /// <param name="reportsCount">The new reports count</param>
        public static void UpdateUserReportsCount(int userId, int reportsCount)
        {
            for (int i = 0; i < _currentUserCount; i++)
            {
                if ((int)_userDataArray[i, 0, _currentTimeIndex] == userId)
                {
                    _userDataArray[i, 5, _currentTimeIndex] = reportsCount;
                    break;
                }
            }
        }

        /// <summary>
        /// Updates user badges count in multi-dimensional array
        /// </summary>
        /// <param name="userId">The user ID to update</param>
        /// <param name="badgesCount">The new badges count</param>
        public static void UpdateUserBadgesCount(int userId, int badgesCount)
        {
            for (int i = 0; i < _currentUserCount; i++)
            {
                if ((int)_userDataArray[i, 0, _currentTimeIndex] == userId)
                {
                    _userDataArray[i, 6, _currentTimeIndex] = badgesCount;
                    break;
                }
            }
        }

        /// <summary>
        /// Deactivates a user account
        /// </summary>
        /// <param name="userId">The ID of the user to deactivate</param>
        /// <returns>True if user was deactivated successfully, false otherwise</returns>
        public bool DeactivateUser(int userId)
        {
            var user = GetUserById(userId);
            if (user != null)
            {
                user.IsActive = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets system statistics for gamification dashboard
        /// </summary>
        /// <returns>A dictionary containing various system statistics</returns>
        public Dictionary<string, object> GetSystemStatistics()
        {
            int activeUsers = 0;
            int totalPoints = 0;
            int totalBadges = 0;
            int totalIssues = 0;
            var levelDistribution = new Dictionary<UserLevel, int>();

            foreach (User user in _users)
            {
                if (user.IsActive)
                {
                    activeUsers++;
                }
                
                totalPoints += user.TotalPoints;
                totalBadges += user.Badges.Count;
                totalIssues += user.ReportedIssues.Count;

                // Count users by level
                if (levelDistribution.ContainsKey(user.Level))
                {
                    levelDistribution[user.Level]++;
                }
                else
                {
                    levelDistribution[user.Level] = 1;
                }
            }

            var stats = new Dictionary<string, object>
            {
                ["TotalUsers"] = _users.Count,
                ["ActiveUsers"] = activeUsers,
                ["TotalPoints"] = totalPoints,
                ["TotalBadges"] = totalBadges,
                ["TotalIssues"] = totalIssues,
                ["LevelDistribution"] = levelDistribution
            };

            return stats;
        }
    }

    /// <summary>
    /// Class for tracking new user registrations
    /// </summary>
    public class NewUserRegistration
    {
        public int UserIndex { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Source { get; set; } = string.Empty;
        public int InitialPoints { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
