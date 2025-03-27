using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;

namespace DACN_VILLA.Repository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Wishlist> GetByIdAsync(Guid id)
        {
            return await _context.Wishlists
                .Include(w => w.Villa)
                .FirstOrDefaultAsync(w => w.Id == id);
        }
        public async Task<IEnumerable<Wishlist>> GetAllWishlistByUserId(Guid userId)
        {
            return await _context.Wishlists
                .Include(b => b.Villa)
                    .ThenInclude(v => v.VillaImages)
                .Include(b => b.User)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Villa)
                .ToListAsync();
        }

        public async Task<Wishlist> GetByUserIdAndVillaIdAsync(Guid userId, Guid villaId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.VillaId == villaId);
        }

        public async Task AddAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Guid id)
        {
            var wishlist = await GetByIdAsync(id);
            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid userId, Guid villaId)
        {
            return await _context.Wishlists.AnyAsync(w => w.UserId == userId && w.VillaId == villaId);
        }
    }
}
