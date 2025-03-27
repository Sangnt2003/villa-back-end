using DACN_VILLA.Model.Enum;
using Microsoft.EntityFrameworkCore;

namespace DACN_VILLA.Interface.Repository
{
    public interface IBookingRepository
    {
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<Booking> GetByIdAsync(Guid id);
        Task<bool> UpdateBookingStatusAndBalanceAsync(Guid bookingId, Guid userId, decimal amount);
        Task AddAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(Guid id);
        Task<Booking> GetBookingByUserIdAndVillaIdAsync(Guid userId, Guid villaId);
        Guid GetVillaIdByBookingId(Guid bookingId);
        Task<IEnumerable<Booking>> GetAllBookingByUserId(Guid userId);
    }
}
