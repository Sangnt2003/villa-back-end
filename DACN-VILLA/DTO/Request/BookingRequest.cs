using DACN_VILLA.Model.Enum;

public class BookingRequest
{
    public Guid UserId { get; set; }
    public Guid VillaId { get; set; }
    public DateTime checkInDate { get; set; }
    public DateTime checkOutDate { get; set; }
    public string checkInHour {  get; set; }
    public string FullName { get; set; }
    public decimal TotalPrice { get; set; }
    public int Capacity { get; set; }
    public ApprovalStatusBooking ApprovalStatus { get; set; } = ApprovalStatusBooking.Pending;
    public string? PaymentMethod { get; set; } 
    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
}
