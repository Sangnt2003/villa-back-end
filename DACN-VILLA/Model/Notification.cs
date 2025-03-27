namespace DACN_VILLA.Model
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid VillaOwnerId {  get; set; }
        public string Title {  get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}
