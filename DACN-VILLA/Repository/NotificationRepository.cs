using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;

namespace DACN_VILLA.Repository
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationRepository> _logger;
        public NotificationRepository(ApplicationDbContext context, ILogger<NotificationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByVillaOwnerIdAsync(Guid villaOwnerId)
        {
            return await _context.Notifications
                                 .Where(n => n.VillaOwnerId == villaOwnerId)
                                 .ToListAsync();
        }

        public async Task<Notification> GetNotificationByIdAsync(Guid id)
        {
            return await _context.Notifications
                                 .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<Notification> GetNotificationByTitleAndMessageAsync(string title, string message)
        {
            return await _context.Notifications
                .Where(n => n.Title == title && n.Message == message)
                .FirstOrDefaultAsync();
        }

        public async Task CreateNotificationAsync(Notification notification)
        {
            _logger.LogInformation("Adding new notification with Id: {NotificationId}", notification.Id);
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Notification with Id: {NotificationId} saved successfully", notification.Id);
        }

        public async Task RemoveDuplicateNotificationsAsync(Notification notification)
        {
            var duplicateNotifications = await _context.Notifications
                .Where(n => n.VillaOwnerId == notification.VillaOwnerId
                            && n.Title == notification.Title
                            && n.Message == notification.Message)
                .ToListAsync();

            if (duplicateNotifications.Count > 1)
            {
                _logger.LogInformation("Found {Count} duplicate notifications for VillaOwnerId: {VillaOwnerId}, Title: {Title}, Message: {Message}",
                    duplicateNotifications.Count, notification.VillaOwnerId, notification.Title, notification.Message);
                var duplicatesToRemove = duplicateNotifications.Skip(1); 
                _context.Notifications.RemoveRange(duplicatesToRemove); 

                await _context.SaveChangesAsync();
                _logger.LogInformation("Removed {Count} duplicate notifications.", duplicatesToRemove.Count());
            }
            else
            {
                // Log nếu không có bản ghi trùng lặp nào
                _logger.LogInformation("No duplicate notifications found for VillaOwnerId: {VillaOwnerId}, Title: {Title}, Message: {Message}",
                    notification.VillaOwnerId, notification.Title, notification.Message);
            }
        }

        public async Task UpdateNotificationAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(Guid id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }
    }

}
