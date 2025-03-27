using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DACN_VILLA.Controllers
{
    [Route("api/review")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(ReviewRequest reviewRequest)
        {
            try
            {
                var review = await _reviewService.AddReviewAsync(reviewRequest);
                return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("villa/{villaId}")]
        public async Task<ActionResult<IEnumerable<ReviewResponse>>> GetReviewsByVillaId(Guid villaId)
        {
            var reviews = await _reviewService.GetReviewsByVillaIdAsync(villaId);
            return Ok(reviews);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReviewResponse>> GetReviewById(Guid id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);
            if (review == null)
                return NotFound();
            return Ok(review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var deleted = await _reviewService.DeleteReviewAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
