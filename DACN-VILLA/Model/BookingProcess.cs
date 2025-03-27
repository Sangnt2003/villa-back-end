using DACN_VILLA.Model.Enum;
using DACN_VILLA.Model;

public class BookingProcess
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid UserId { get; set; }
    public ApprovalStatusBooking ApprovalStatus { get; set; } = ApprovalStatusBooking.Pending;
    public DateTime ProcessedAt { get; set; } = DateTime.Now;
    public string Note { get; set; }
    public Booking Booking { get; set; }
    public User User { get; set; }
}
