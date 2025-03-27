using DACN_VILLA.DTO.Response;

namespace DACN_VILLA.DTO.Respone
{
    public class WishlistListResponse
    {
        public List<WishlistResponse> Wishlists { get; set; }
        public int TotalPages { get; set; }
    }
}
