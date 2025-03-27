using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.DTO.Response;
using DACN_VILLA.Model;

namespace DACN_VILLA.Interface.Service
{
    public interface IWishListService
    {
        /*Task<WishlistResponse> GetWishlistsByUserIdAsync(Guid userId);*/
        Task<WishlistResponse> AddToWishlistAsync(WishlistCreateRequest request);
        Task RemoveFromWishlistAsync(Guid id);
        Task<WishlistResponse?> CheckWishlistAsync(Guid userId, Guid villaId);

        Task<IEnumerable<WishlistResponse>> GetWishlistByUserIdAsync(Guid userId);

    }
}
