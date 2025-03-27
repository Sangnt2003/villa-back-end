using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DACN_VILLA.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Thêm review mới
        public async Task<Review> AddReviewAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        // Lấy review theo ID
        public async Task<Review> GetReviewByIdAsync(Guid id)
        {
            return await _context.Reviews
                                 .Include(r => r.Villa)
                                 .Include(r => r.Customer)
                                 .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Lấy tất cả review của một villa
        public async Task<IEnumerable<Review>> GetReviewsByVillaIdAsync(Guid villaId)
        {
            return await _context.Reviews
                                 .Where(r => r.VillaId == villaId && !r.IsDeleted)
                                 .Include(r => r.Customer)
                                 .ToListAsync();
        }

        // Xóa review
        public async Task<bool> DeleteReviewAsync(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return false;

            review.IsDeleted = true; // Đánh dấu review là đã xóa thay vì xóa vĩnh viễn.
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
