using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace DACN_VILLA.Repository
{
    public class BookingProcessRepository : IBookingProcessRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingProcessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookingProcess>> GetAllAsync()
        {
            return await _context.BookingProcesses.ToListAsync();
        }

        public async Task<BookingProcess> GetByIdAsync(Guid id)
        {
            return await _context.BookingProcesses.FindAsync(id);
        }

        public async Task AddAsync(BookingProcess bookingProcess)
        {
            _context.BookingProcesses.Add(bookingProcess);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BookingProcess bookingProcess)
        {
            _context.BookingProcesses.Update(bookingProcess);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var bookingProcess = await _context.BookingProcesses.FindAsync(id);
            if (bookingProcess != null)
            {
                _context.BookingProcesses.Remove(bookingProcess);
                await _context.SaveChangesAsync();
            }
        }
    }

}
