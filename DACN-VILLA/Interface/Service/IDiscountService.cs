using DACN_VILLA.DTO.Respone;

namespace DACN_VILLA.Interface.Service
{
    public interface IDiscountService
    {
        Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync();
        Task<DiscountResponse> GetDiscountByIdAsync(Guid id);
        Task AddDiscountAsync(DiscountResponse discountDto);
        Task UpdateDiscountAsync(DiscountResponse discountDto);
        Task DeleteDiscountAsync(Guid id);
        Task<decimal> GetDiscountPercentageAsync(Guid discountId);
    }
}
