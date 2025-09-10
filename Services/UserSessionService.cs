using MunicipalServicesApp.Models;
using MunicipalServicesApp.Data;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Service for managing user sessions and authentication state
    /// </summary>
    public class UserSessionService
    {
        private readonly UserRepository _userRepository;
        private readonly DataService _dataService;

        public UserSessionService(UserRepository userRepository, DataService dataService)
        {
            _userRepository = userRepository;
            _dataService = dataService;
        }

        /// <summary>
        /// Get current user from session
        /// </summary>
        public User? GetCurrentUser(HttpContext httpContext)
        {
            var userIdString = httpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return null;
            }

            return _userRepository.GetUserById(userId);
        }

        /// <summary>
        /// Get current user session (for demo purposes)
        /// </summary>
        public UserSession GetCurrentUserSession(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            if (user != null)
            {
                return _dataService.GetOrCreateUserSession(user.Email, user.Username, user.Municipality);
            }

            // Return demo session if no user is logged in
            return _dataService.GetOrCreateUserSession("demo@municipal.gov.za", "Demo User", "Cape Town");
        }

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        public bool IsUserLoggedIn(HttpContext httpContext)
        {
            return !string.IsNullOrEmpty(httpContext.Session.GetString("UserId"));
        }

        /// <summary>
        /// Get current user's email (for notifications and data)
        /// </summary>
        public string GetCurrentUserEmail(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            return user?.Email ?? "demo@municipal.gov.za";
        }

        /// <summary>
        /// Get current user's display name
        /// </summary>
        public string GetCurrentUserDisplayName(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            return user?.Username ?? "Demo User";
        }

        /// <summary>
        /// Get current user's level
        /// </summary>
        public UserLevel GetCurrentUserLevel(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            return user?.Level ?? UserLevel.Bronze;
        }

        /// <summary>
        /// Get current user's total points
        /// </summary>
        public int GetCurrentUserPoints(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            return user?.TotalPoints ?? 0;
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        public void Logout(HttpContext httpContext)
        {
            httpContext.Session.Clear();
        }

        /// <summary>
        /// Login user and set session
        /// </summary>
        public bool LoginUser(HttpContext httpContext, string username, string password)
        {
            var user = _userRepository.LoginUser(username, password);
            if (user != null)
            {
                httpContext.Session.SetString("UserId", user.Id.ToString());
                httpContext.Session.SetString("Username", user.Username);
                httpContext.Session.SetString("UserEmail", user.Email);
                httpContext.Session.SetString("UserLevel", user.Level.ToString());
                httpContext.Session.SetString("UserPoints", user.TotalPoints.ToString());
                httpContext.Session.SetString("UserMunicipality", user.Municipality);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get user session info for display
        /// </summary>
        public UserSessionInfo GetUserSessionInfo(HttpContext httpContext)
        {
            var user = GetCurrentUser(httpContext);
            var session = GetCurrentUserSession(httpContext);

            return new UserSessionInfo
            {
                IsLoggedIn = IsUserLoggedIn(httpContext),
                UserId = user?.Id ?? 0,
                Username = user?.Username ?? "Demo User",
                Email = user?.Email ?? "demo@municipal.gov.za",
                Level = user?.Level ?? UserLevel.Bronze,
                TotalPoints = user?.TotalPoints ?? session.TotalPoints,
                Municipality = user?.Municipality ?? session.Municipality,
                ReportsSubmitted = session.ReportsSubmitted,
                EarnedBadgesCount = session.EarnedBadges.Count,
                LastActiveAt = session.LastActiveAt
            };
        }
    }

    /// <summary>
    /// User session information for display
    /// </summary>
    public class UserSessionInfo
    {
        public bool IsLoggedIn { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserLevel Level { get; set; }
        public int TotalPoints { get; set; }
        public string Municipality { get; set; } = string.Empty;
        public int ReportsSubmitted { get; set; }
        public int EarnedBadgesCount { get; set; }
        public DateTime LastActiveAt { get; set; }
    }
}
