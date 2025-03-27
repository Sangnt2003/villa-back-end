using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;

namespace DACN_VILLA.Interface.Service
{
    public interface IReviewService
    {
        Task<ReviewResponse> AddReviewAsync(ReviewRequest reviewRequest);
        Task<ReviewResponse> GetReviewByIdAsync(Guid id);
        Task<bool> DeleteReviewAsync(Guid id);
        Task<IEnumerable<ReviewResponse>> GetReviewsByVillaIdAsync(Guid villaId);
    }
}
