using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using DACN_VILLA.Model.Enum;
using Docker.DotNet.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace DACN_VILLA.Repository
{
    public class BookingRepository : IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> UpdateBookingStatusAndBalanceAsync(Guid bookingId, Guid userId, decimal amount)
        {
            // Lấy booking dựa vào bookingId
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return false;

            // Cập nhật trạng thái booking
            booking.ApprovalStatus = (ApprovalStatusBooking)(int)ApprovalStatusBooking.Complete;

            // Lấy thông tin user dựa vào userId
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) throw new ArgumentException("Không tìm thấy user với userId đã chỉ định.");

            // Cộng amount vào Balance của user
            user.Balance += amount;

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<Booking> GetByIdAsync(Guid id)
        {
            return await _context.Bookings.Include(v => v.Villa).FirstOrDefaultAsync(v => v.Id == id);
        }
        public async Task<IEnumerable<Booking>> GetAllBookingByUserId(Guid userId)
        {
            return await _context.Bookings
                .Include(b => b.Villa) 
                    .ThenInclude(v => v.Location) 
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }

        public Guid GetVillaIdByBookingId(Guid bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            return booking.VillaId;
        }
        public async Task<Booking> GetBookingByUserIdAndVillaIdAsync(Guid userId, Guid villaId)
        {
            return await _context.Bookings
                                 .FirstOrDefaultAsync(b => b.UserId == userId && b.VillaId == villaId && b.ApprovalStatus == ApprovalStatusBooking.Complete);
        }
        public async Task DeleteAsync(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
        }
    }

}
