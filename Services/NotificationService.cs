using MunicipalServicesApp.Models;
using System.Collections.Concurrent;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Notification Service for managing user feedback and system alerts
    /// Implements proper event handling and notification management
    /// </summary>
    public class NotificationService
    {
        private readonly ConcurrentDictionary<string, List<Notification>> _userNotifications;
        private readonly ConcurrentQueue<Notification> _notificationQueue;
        private readonly object _lockObject = new object();

        // Events for real-time notifications
        public event EventHandler<NotificationEventArgs>? NotificationCreated;
        public event EventHandler<NotificationEventArgs>? NotificationRead;
        public event EventHandler<BadgeEarningNotificationEventArgs>? BadgeEarningNotification;

        public NotificationService()
        {
            _userNotifications = new ConcurrentDictionary<string, List<Notification>>();
            _notificationQueue = new ConcurrentQueue<Notification>();
        }

        /// <summary>
        /// Create a new notification with proper event handling
        /// </summary>
        public async Task<Notification> CreateNotificationAsync(Notification notification)
        {
            notification.Id = Guid.NewGuid();
            notification.CreatedAt = DateTime.UtcNow;
            notification.IsActive = true;

            // Add to user's notification list
            _userNotifications.AddOrUpdate(
                notification.UserEmail,
                new List<Notification> { notification },
                (key, existing) => { existing.Add(notification); return existing; });

            // Add to queue for processing
            _notificationQueue.Enqueue(notification);

            // Raise event
            OnNotificationCreated(new NotificationEventArgs(notification));

            // Special handling for badge earning notifications
            if (notification.Type == NotificationType.BadgeEarned)
            {
                OnBadgeEarningNotification(new BadgeEarningNotificationEventArgs(notification));
            }

            return notification;
        }

        /// <summary>
        /// Create a badge earning notification with enhanced details
        /// </summary>
        public async Task<Notification> CreateBadgeEarningNotificationAsync(BadgeEarningNotification badgeNotification)
        {
            var notification = new Notification
            {
                UserEmail = badgeNotification.UserEmail,
                Type = NotificationType.BadgeEarned,
                Title = "🎉 Badge Earned!",
                Message = badgeNotification.AchievementMessage,
                ImagePath = badgeNotification.Badge.ImagePath,
                PointsAwarded = badgeNotification.PointsAwarded,
                RelatedBadgeId = badgeNotification.Badge.Id,
                CreatedAt = badgeNotification.EarnedAt
            };

            return await CreateNotificationAsync(notification);
        }

        /// <summary>
        /// Get notifications for a specific user
        /// </summary>
        public List<Notification> GetUserNotifications(string userEmail, bool unreadOnly = false)
        {
            if (_userNotifications.TryGetValue(userEmail, out var notifications))
            {
                var filtered = unreadOnly 
                    ? notifications.Where(n => !n.IsRead && n.IsActive).ToList()
                    : notifications.Where(n => n.IsActive).ToList();
                
                return filtered.OrderByDescending(n => n.CreatedAt).ToList();
            }

            return new List<Notification>();
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        public async Task<bool> MarkAsReadAsync(Guid notificationId, string userEmail)
        {
            if (_userNotifications.TryGetValue(userEmail, out var notifications))
            {
                var notification = notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    
                    OnNotificationRead(new NotificationEventArgs(notification));
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Mark all notifications as read for a user
        /// </summary>
        public async Task<int> MarkAllAsReadAsync(string userEmail)
        {
            if (_userNotifications.TryGetValue(userEmail, out var notifications))
            {
                var unreadCount = 0;
                foreach (var notification in notifications.Where(n => !n.IsRead && n.IsActive))
                {
                    notification.IsRead = true;
                    notification.ReadAt = DateTime.UtcNow;
                    unreadCount++;
                }
                return unreadCount;
            }

            return 0;
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        public async Task<bool> DeleteNotificationAsync(Guid notificationId, string userEmail)
        {
            if (_userNotifications.TryGetValue(userEmail, out var notifications))
            {
                var notification = notifications.FirstOrDefault(n => n.Id == notificationId);
                if (notification != null)
                {
                    notification.IsActive = false;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get notification statistics for a user
        /// </summary>
        public NotificationStats GetNotificationStats(string userEmail)
        {
            if (_userNotifications.TryGetValue(userEmail, out var notifications))
            {
                var activeNotifications = notifications.Where(n => n.IsActive).ToList();
                
                return new NotificationStats
                {
                    TotalNotifications = activeNotifications.Count,
                    UnreadNotifications = activeNotifications.Count(n => !n.IsRead),
                    BadgeNotifications = activeNotifications.Count(n => n.Type == NotificationType.BadgeEarned),
                    IssueNotifications = activeNotifications.Count(n => n.Type == NotificationType.IssueSubmitted || 
                                                                      n.Type == NotificationType.IssueStatusChanged),
                    SystemNotifications = activeNotifications.Count(n => n.Type == NotificationType.SystemAlert),
                    LastNotificationDate = activeNotifications.Any() ? activeNotifications.Max(n => n.CreatedAt) : DateTime.MinValue
                };
            }

            return new NotificationStats();
        }

        /// <summary>
        /// Get recent notifications for dashboard display
        /// </summary>
        public List<Notification> GetRecentNotifications(string userEmail, int count = 5)
        {
            return GetUserNotifications(userEmail)
                .Take(count)
                .ToList();
        }

        /// <summary>
        /// Process notification queue (for background processing)
        /// </summary>
        public async Task ProcessNotificationQueueAsync()
        {
            var processedCount = 0;
            var maxProcess = 100; // Process up to 100 notifications at a time

            while (_notificationQueue.TryDequeue(out var notification) && processedCount < maxProcess)
            {
                // Here you would implement actual notification delivery
                // (email, SMS, push notifications, etc.)
                await ProcessNotificationAsync(notification);
                processedCount++;
            }
        }

        /// <summary>
        /// Process individual notification (placeholder for actual delivery)
        /// </summary>
        private async Task ProcessNotificationAsync(Notification notification)
        {
            // Simulate processing delay
            await Task.Delay(100);
            
            // In a real implementation, this would:
            // - Send email notifications
            // - Send push notifications
            // - Send SMS alerts
            // - Update real-time UI components
            // - Log to analytics systems
        }

        /// <summary>
        /// Create a badge earning notification with celebration message
        /// </summary>
        public async Task<Notification> CreateBadgeCelebrationNotificationAsync(Badge badge, string userEmail, int pointsAwarded)
        {
            var celebrationMessages = new[]
            {
                $"🎉 Congratulations! You've earned the '{badge.Name}' badge!",
                $"🏆 Amazing work! The '{badge.Name}' badge is now yours!",
                $"⭐ Outstanding! You've unlocked the '{badge.Name}' badge!",
                $"🎊 Fantastic! You've achieved the '{badge.Name}' badge!",
                $"🌟 Excellent! The '{badge.Name}' badge is yours to keep!"
            };

            var random = new Random();
            var message = celebrationMessages[random.Next(celebrationMessages.Length)];

            var notification = new Notification
            {
                UserEmail = userEmail,
                Type = NotificationType.BadgeEarned,
                Title = "Badge Earned! 🎉",
                Message = message,
                ImagePath = badge.ImagePath,
                PointsAwarded = pointsAwarded,
                RelatedBadgeId = badge.Id
            };

            return await CreateNotificationAsync(notification);
        }

        #region Event Handlers

        protected virtual void OnNotificationCreated(NotificationEventArgs e)
        {
            NotificationCreated?.Invoke(this, e);
        }

        protected virtual void OnNotificationRead(NotificationEventArgs e)
        {
            NotificationRead?.Invoke(this, e);
        }

        protected virtual void OnBadgeEarningNotification(BadgeEarningNotificationEventArgs e)
        {
            BadgeEarningNotification?.Invoke(this, e);
        }

        #endregion
    }

    #region Event Args Classes

    public class NotificationEventArgs : EventArgs
    {
        public Notification Notification { get; }
        public DateTime Timestamp { get; }

        public NotificationEventArgs(Notification notification)
        {
            Notification = notification;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class BadgeEarningNotificationEventArgs : EventArgs
    {
        public Notification Notification { get; }
        public Badge Badge { get; }
        public DateTime Timestamp { get; }

        public BadgeEarningNotificationEventArgs(Notification notification)
        {
            Notification = notification;
            Badge = new Badge(); // Would be populated from notification
            Timestamp = DateTime.UtcNow;
        }
    }

    #endregion
}
