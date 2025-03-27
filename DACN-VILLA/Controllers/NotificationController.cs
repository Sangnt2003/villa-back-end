using DACN_VILLA.DTO.Request;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Service;
using Microsoft.AspNetCore.Mvc;

namespace DACN_VILLA.Controllers
{
    [Route("api/notification")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationController> _logger;
        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet("owner/{villaOwnerId}")]
        public async Task<IActionResult> GetNotificationsByVillaOwnerId(Guid villaOwnerId)
        {
            var notifications = await _notificationService.GetNotificationsByVillaOwnerIdAsync(villaOwnerId);
            return Ok(notifications);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationById(Guid id)
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            return Ok(notification);
        }

        [HttpPost]
        public async Task<ActionResult> CreateNotification([FromBody] NotificationRequest notificationRequest)
        {
            var createNotification = await _notificationService.CreateNotificationAsync(notificationRequest);
            return CreatedAtAction(nameof(GetNotificationById), new { id = createNotification.Id }, createNotification);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNotification(Guid id, [FromBody] NotificationRequest notificationRequest)
        {
            await _notificationService.UpdateNotificationAsync(id, notificationRequest);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            await _notificationService.DeleteNotificationAsync(id);
            return NoContent();
        }
    }
}
