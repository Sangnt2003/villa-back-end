using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using DACN_VILLA.Model.Enum;
using Microsoft.EntityFrameworkCore;
using System;

namespace DACN_VILLA.Repository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly ApplicationDbContext _context;

        public LocationRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<LocationWithVillaCount>> GetLocationsWithApprovedVillaCountAsync()
        {
            var locationsWithVillas = await _context.Locations
                .Where(l => l.Villas.Any(v => v.ApprovalStatus == ApprovalStatus.Approved)) 
                .Select(l => new
                {
                    Location = l,
                    ApprovedVillaCount = l.Villas.Count(v => v.ApprovalStatus == ApprovalStatus.Approved)
                })
                .ToListAsync();
            var villaBookings = await _context.Bookings
                .Where(b => b.Villa.ApprovalStatus == ApprovalStatus.Approved && b.ApprovalStatus == ApprovalStatusBooking.Complete || b.ApprovalStatus == ApprovalStatusBooking.Canceled)
                .GroupBy(b => b.Villa.LocationId) 
                .Select(g => new
                {
                    LocationId = g.Key,
                    ApprovedBookingCount = g.Count() 
                })
                .ToListAsync();

            // Combine both results
            var result = locationsWithVillas.Select(lv =>
            {
                var bookings = villaBookings.FirstOrDefault(b => b.LocationId == lv.Location.Id);
                return new LocationWithVillaCount
                {
                    Id = lv.Location.Id,
                    Name = lv.Location.Name,
                    Description = lv.Location.Description,
                    ImageUrl = lv.Location.ImageUrl,
                    TotalVilla = lv.ApprovedVillaCount,
                    TotalBooking = bookings?.ApprovedBookingCount ?? 0
                };
            }).ToList();

            return result;
        }


        public async Task<IEnumerable<Location>> GetAllAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location> GetByIdAsync(Guid id)
        {
            return await _context.Locations.FindAsync(id);
        }

        public async Task AddAsync(Location location)
        {
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Location location)
        {
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();
            }
        }
    }

}
