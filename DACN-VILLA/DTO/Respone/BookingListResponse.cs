using DACN_VILLA.DTO.Response;

namespace DACN_VILLA.DTO.Respone
{
    public class BookingListResponse
    {
        public List<BookingResponse> Bookings { get; set; }
        public int TotalPages { get; set; }
    }
}
