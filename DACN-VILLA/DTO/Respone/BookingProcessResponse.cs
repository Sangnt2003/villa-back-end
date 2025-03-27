using DACN_VILLA.Model.Enum;

namespace DACN_VILLA.DTO.Response 
{
    public class BookingProcessResponse
    {
        public DateTime ProcessedAt { get; set; }
        public string Note { get; set; }
        public ApprovalStatusBooking ApprovalStatus { get; set; } = ApprovalStatusBooking.Pending;
    }
}
