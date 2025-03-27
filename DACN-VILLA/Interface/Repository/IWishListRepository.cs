using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;

public interface IWishlistRepository
{
    Task<Wishlist> GetByIdAsync(Guid id);
    Task<IEnumerable<Wishlist>> GetByUserIdAsync(Guid userId);
    Task<Wishlist> GetByUserIdAndVillaIdAsync(Guid userId, Guid villaId);
    Task AddAsync(Wishlist wishlist);
    Task RemoveAsync(Guid id);
    Task<bool> ExistsAsync(Guid userId, Guid villaId);
    Task<IEnumerable<Wishlist>> GetAllWishlistByUserId(Guid userId);
}
