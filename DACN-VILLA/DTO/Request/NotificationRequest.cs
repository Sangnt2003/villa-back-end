namespace DACN_VILLA.DTO.Request
{
    public class NotificationRequest
    {
        public Guid VillaOwnerId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

    }
}
