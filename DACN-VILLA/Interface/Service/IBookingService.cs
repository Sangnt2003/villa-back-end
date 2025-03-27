using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.DTO.Response;
using DACN_VILLA.Model.Enum;
using Microsoft.AspNetCore.Mvc;

namespace DACN_VILLA.Interface.Service
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(BookingRequest request);
        Task<bool> CancelBookingAsync(Guid bookingId);
        Task<bool> RefundToOwnerAsync();
        Task<IEnumerable<BookingResponse>> GetAllBookingAsync();
        Task<BookingResponse> GetBookingByIdAsync(Guid bookingId);
        Task<IEnumerable<BookingResponse>> GetBookingByUserIdAsync(Guid userId);
        Task<bool> UpdateBookingStatusAsync(Guid bookingId);
    }
}
