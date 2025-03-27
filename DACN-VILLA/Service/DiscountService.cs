using AutoMapper;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;

namespace DACN_VILLA.Service
{
    public class DiscountService : IDiscountService
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository discountRepository, IMapper mapper)
        {
            _discountRepository = discountRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DiscountResponse>> GetAllDiscountsAsync()
        {
            var discounts = await _discountRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DiscountResponse>>(discounts);
        }

        public async Task<DiscountResponse> GetDiscountByIdAsync(Guid id)
        {
            var discount = await _discountRepository.GetByIdAsync(id);
            return _mapper.Map<DiscountResponse>(discount);
        }

        public async Task AddDiscountAsync(DiscountResponse discountDto)
        {
            var discount = _mapper.Map<Discount>(discountDto);
            await _discountRepository.AddAsync(discount);
        }

        public async Task UpdateDiscountAsync(DiscountResponse discountDto)
        {
            var discount = _mapper.Map<Discount>(discountDto);
            await _discountRepository.UpdateAsync(discount);
        }

        public async Task DeleteDiscountAsync(Guid id)
        {
            await _discountRepository.DeleteAsync(id);
        }

        public async Task<decimal> GetDiscountPercentageAsync(Guid discountId)
        {
            var discount = await _discountRepository.GetByIdAsync(discountId);
            return discount?.Percentage ?? 0;
        }
    }
}
