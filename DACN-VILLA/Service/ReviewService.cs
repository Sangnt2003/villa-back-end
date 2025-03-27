using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using DACN_VILLA.Model.Enum;
using DACN_VILLA.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DACN_VILLA.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IBookingRepository _bookingRepository;

        public ReviewService(IReviewRepository reviewRepository, IBookingRepository bookingRepository)
        {
            _reviewRepository = reviewRepository;
            _bookingRepository = bookingRepository;
        }

        public async Task<ReviewResponse> AddReviewAsync(ReviewRequest reviewRequest)
        {
            // Kiểm tra xem người dùng đã đặt thành công villa chưa
            var booking = await _bookingRepository.GetBookingByUserIdAndVillaIdAsync(reviewRequest.UserId, reviewRequest.VillaId);
            if (booking == null || booking.ApprovalStatus != ApprovalStatusBooking.Complete)
            {
                throw new InvalidOperationException("User must have a successful booking to leave a review.");
            }

            var review = new Review
            {
                Id = Guid.NewGuid(),
                VillaId = reviewRequest.VillaId,
                CustomerId = reviewRequest.UserId,
                Rating = reviewRequest.Rating,
                Comment = reviewRequest.Comment,
                PreCmtId = reviewRequest.PreCmtId,
                CreatedAt = DateTime.UtcNow,
            };

            var addedReview = await _reviewRepository.AddReviewAsync(review);

            return new ReviewResponse
            {
                Id = addedReview.Id,
                VillaId = addedReview.VillaId,
                UserId = addedReview.CustomerId,
                Rating = addedReview.Rating,
                Comment = addedReview.Comment,
                CreatedAt = addedReview.CreatedAt
            };
        }

        public async Task<ReviewResponse> GetReviewByIdAsync(Guid id)
        {
            var review = await _reviewRepository.GetReviewByIdAsync(id);
            if (review == null)
                return null;

            return new ReviewResponse
            {
                Id = review.Id,
                VillaId = review.VillaId,
                UserId = review.CustomerId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }

        public async Task<bool> DeleteReviewAsync(Guid id)
        {
            return await _reviewRepository.DeleteReviewAsync(id);
        }

        public async Task<IEnumerable<ReviewResponse>> GetReviewsByVillaIdAsync(Guid villaId)
        {
            var reviews = await _reviewRepository.GetReviewsByVillaIdAsync(villaId);
            return reviews.Select(r => new ReviewResponse
            {
                Id = r.Id,
                VillaId = r.VillaId,
                UserId = r.CustomerId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            });
        }
    }
}
