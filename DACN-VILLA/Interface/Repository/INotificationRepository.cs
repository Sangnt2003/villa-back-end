using DACN_VILLA.Model;

namespace DACN_VILLA.Interface.Repository
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetNotificationsByVillaOwnerIdAsync(Guid villaOwnerId);
        Task<Notification> GetNotificationByIdAsync(Guid id);
        Task CreateNotificationAsync(Notification notification);
        Task RemoveDuplicateNotificationsAsync(Notification notification);
        Task UpdateNotificationAsync(Notification notification);
        Task<Notification> GetNotificationByTitleAndMessageAsync(string title, string message);
        Task DeleteNotificationAsync(Guid id);
    }

}
