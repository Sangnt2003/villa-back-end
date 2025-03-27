using DACN_VILLA.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DACN_VILLA.Repository
{
    public interface IReviewRepository
    {
        Task<Review> AddReviewAsync(Review review);
        Task<Review> GetReviewByIdAsync(Guid id);
        Task<IEnumerable<Review>> GetReviewsByVillaIdAsync(Guid villaId);
        Task<bool> DeleteReviewAsync(Guid id);
    }
}
