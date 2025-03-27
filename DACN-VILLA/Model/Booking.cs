using DACN_VILLA.Model;
using DACN_VILLA.Model.Enum;
using System.ComponentModel.DataAnnotations.Schema;

public class Booking
{
    public Guid Id { get; set; }
    public Guid VillaId { get; set; }
    public Guid UserId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string? CheckInHour {  get; set; }
    public decimal TotalPrice { get; set; }
    public ApprovalStatusBooking ApprovalStatus { get; set; } = ApprovalStatusBooking.Pending;
    public DateTime BookingDate {  get; set; }
    public int NumberOfGuests { get; set; }
    public Villa Villa { get; set; }
    public User User { get; set; }
    public ICollection<BookingProcess> BookingProcesses { get; set; } = new List<BookingProcess>();
}
