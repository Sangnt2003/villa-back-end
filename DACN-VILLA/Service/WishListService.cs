using AutoMapper;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.DTO.Response;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using DACN_VILLA.Repository;
using Microsoft.EntityFrameworkCore;

namespace DACN_VILLA.Service
{
    public class WishlistService : IWishListService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly IVillaRepository _villaRepository;
        private ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public WishlistService(
            IWishlistRepository wishlistRepository, 
            IVillaRepository villaRepository, 
            IMapper mapper,
            ApplicationDbContext context)
        {
            _wishlistRepository = wishlistRepository;
            _villaRepository = villaRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<IEnumerable<WishlistResponse>> GetWishlistByUserIdAsync(Guid userId)
        {
            var wishlists = await _wishlistRepository.GetAllWishlistByUserId(userId);
            return _mapper.Map<IEnumerable<WishlistResponse>>(wishlists);
        }

        public async Task<WishlistResponse> AddToWishlistAsync(WishlistCreateRequest request)
        {
            if (await _wishlistRepository.GetByUserIdAndVillaIdAsync(request.UserId, request.VillaId) != null)
            {
                throw new Exception("Villa already in wishlist.");
            }
            if (await _villaRepository.GetByIdAsync(request.VillaId) == null)
            {
                throw new Exception("Villa not found.");
            }
            var wishlist = _mapper.Map<Wishlist>(request);
            await _wishlistRepository.AddAsync(wishlist);

            return _mapper.Map<WishlistResponse>(wishlist);
        }

        public async Task<WishlistResponse?> CheckWishlistAsync(Guid userId, Guid villaId)
        {
            var wishlistItem = await _context.Wishlists
                .Where(w => w.UserId == userId && w.VillaId == villaId)
                .FirstOrDefaultAsync();

            return wishlistItem != null ? new WishlistResponse
            {
                Id = wishlistItem.Id,
                UserId = wishlistItem.UserId,
                VillaId = wishlistItem.VillaId
            } : null;
        }

        public async Task RemoveFromWishlistAsync(Guid id)
        {
            await _wishlistRepository.RemoveAsync(id);
        }
    }
}
