using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Model;

namespace DACN_VILLA.Interface.Service
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponse>> GetNotificationsByVillaOwnerIdAsync(Guid villaOwnerId);
        Task<NotificationResponse> GetNotificationByIdAsync(Guid id);
        Task<NotificationResponse> CreateNotificationAsync(NotificationRequest notificationRequest);
        Task UpdateNotificationAsync(Guid id, NotificationRequest notificationRequest);
        Task DeleteNotificationAsync(Guid id);
    }

}
