using DACN_VILLA.Model;

namespace DACN_VILLA.Interface.Repository
{
    public interface IDiscountRepository
    {
        Task<IEnumerable<Discount>> GetAllAsync();
        Task<Discount> GetByIdAsync(Guid id);
        Task AddAsync(Discount discount);
        Task UpdateAsync(Discount discount);
        Task DeleteAsync(Guid id);
        Task<decimal> GetDiscountPercentageAsync(Guid discountId);
    }
}
