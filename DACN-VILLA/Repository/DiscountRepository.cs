using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;

namespace DACN_VILLA.Repository
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly ApplicationDbContext _context;

        public DiscountRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Discount>> GetAllAsync()
        {
            return await _context.Discounts.ToListAsync();
        }

        public async Task<Discount> GetByIdAsync(Guid id)
        {
            return await _context.Discounts.FindAsync(id);
        }

        public async Task AddAsync(Discount discount)
        {
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Discount discount)
        {
            _context.Discounts.Update(discount);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var discount = await _context.Discounts.FindAsync(id);
            if (discount != null)
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<decimal> GetDiscountPercentageAsync(Guid discountId)
        {
            var discount = await GetByIdAsync(discountId);
            return discount?.Percentage ?? 0;
        }
    }
}
