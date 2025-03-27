using DACN_VILLA.Model;

namespace DACN_VILLA.Interface.Repository
{
    public interface IBookingProcessRepository
    {
        Task<IEnumerable<BookingProcess>> GetAllAsync();
        Task<BookingProcess> GetByIdAsync(Guid id);
        Task AddAsync(BookingProcess bookingProcess);
        Task UpdateAsync(BookingProcess bookingProcess);
        Task DeleteAsync(Guid id);
    }

}
