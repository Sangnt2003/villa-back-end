using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Service;
using Microsoft.AspNetCore.Mvc;

namespace DACN_VILLA.Controllers
{
    [Route("api/wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {
        private readonly IWishListService _wishlistService;

        public WishlistController(IWishListService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        /// <summary>
        /// Gets a paginated list of wishlists by user ID.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="pageNumber">The page number (default is 1).</param>
        /// <param name="pageSize">The number of items per page (default is 7).</param>
        /// <returns>A paginated response of wishlists.</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<WishlistListResponse>> GetWishlistsByUserId(Guid userId, int pageNumber = 1, int pageSize = 7)
        {
            var wishlistResponses = await _wishlistService.GetWishlistByUserIdAsync(userId);

            int totalWishlists = wishlistResponses.Count();

            var pagedWishlists = wishlistResponses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new WishlistListResponse
            {
                Wishlists = pagedWishlists,
                TotalPages = (int)Math.Ceiling((double)totalWishlists / pageSize)
            };

            return Ok(response);
        }

        /// <summary>
        /// Adds a new item to the user's wishlist.
        /// </summary>
        /// <param name="request">The request containing wishlist details.</param>
        /// <returns>The result of the operation.</returns>
        [HttpPost]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistCreateRequest request)
        {
            try
            {
                var response = await _wishlistService.AddToWishlistAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Removes an item from the user's wishlist by its ID.
        /// </summary>
        /// <param name="id">The ID of the wishlist item to remove.</param>
        /// <returns>No content response if successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromWishlist(Guid id)
        {
            try
            {
                await _wishlistService.RemoveFromWishlistAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        /// <summary>
        /// Checks if a villa is already in the user's wishlist.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="villaId">The ID of the villa.</param>
        /// <returns>Whether the villa exists in the wishlist and its ID if applicable.</returns>
        [HttpGet("check")]
        public async Task<IActionResult> CheckWishlist([FromQuery] Guid userId, [FromQuery] Guid villaId)
        {
            try
            {
                var exists = await _wishlistService.CheckWishlistAsync(userId, villaId);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
