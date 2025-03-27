using AutoMapper;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using DACN_VILLA.Repository;

namespace DACN_VILLA.Service
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(INotificationRepository notificationRepository, IMapper mapper, ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<NotificationResponse>> GetNotificationsByVillaOwnerIdAsync(Guid villaOwnerId)
        {
            var notifications = await _notificationRepository.GetNotificationsByVillaOwnerIdAsync(villaOwnerId);
            return _mapper.Map<IEnumerable<NotificationResponse>>(notifications);
        }

        public async Task<NotificationResponse> GetNotificationByIdAsync(Guid id)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(id);
            return _mapper.Map<NotificationResponse>(notification);
        }

        public async Task<NotificationResponse> CreateNotificationAsync(NotificationRequest notificationRequest)
        {
            // Tạo thông báo mới từ NotificationRequest
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                VillaOwnerId = notificationRequest.VillaOwnerId,
                Title = notificationRequest.Title,
                Message = notificationRequest.Message,
                CreatedAt = DateTime.UtcNow
            };

            // Lưu thông báo mới vào cơ sở dữ liệu
            await _notificationRepository.CreateNotificationAsync(notification);
            await _notificationRepository.RemoveDuplicateNotificationsAsync(notification);
            // Trả về thông tin của thông báo
            return new NotificationResponse
            {
                Id = notification.Id,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };
        }



        public async Task UpdateNotificationAsync(Guid id, NotificationRequest notificationRequest)
        {
            var notification = await _notificationRepository.GetNotificationByIdAsync(id);
            if (notification != null)
            {
                _mapper.Map(notificationRequest, notification);
                await _notificationRepository.UpdateNotificationAsync(notification);
            }
        }

        public async Task DeleteNotificationAsync(Guid id)
        {
            await _notificationRepository.DeleteNotificationAsync(id);
        }
    }

}
