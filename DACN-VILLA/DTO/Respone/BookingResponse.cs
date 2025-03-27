using DACN_VILLA.DTO.Response;
using DACN_VILLA.Model.Enum;

public class BookingResponse
{
    public Guid BookingId { get; set; }
    public string VillaName { get; set; }
    public Guid VillaId { get; set; }
    public string VillaLocation { get; set; }
    public string FullName { get; set; }
    public string UserEmail { get; set; }
    public Guid UserId { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string PaymentUrl {  get; set; }
    public decimal TotalPrice { get; set; }
    public decimal DepositAmount { get; set; } // Số tiền đặt cọc
    public decimal RemainingAmount { get; set; } // Số tiền còn lại
    public int NumberOfGuests { get; set; }
    public bool Success { get; set; }
    public string RedirectUrl {  get; set; }
    public string Message {  get; set; }
    public ApprovalStatusBooking ApprovalStatus { get; set; } = ApprovalStatusBooking.Pending;
    public string BookingStatus { get; set; } // Trạng thái đặt phòng (Pending/Completed/Cancelled, ...)
    public DateTime CreatedAt { get; set; } // Ngày tạo
}
