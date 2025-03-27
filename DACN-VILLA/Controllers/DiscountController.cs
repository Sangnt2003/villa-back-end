using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Service;
using Microsoft.AspNetCore.Mvc;

namespace DACN_VILLA.Controllers
{
    [ApiController]
    [Route("api/discount")]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpGet("discounts")]
        public async Task<ActionResult<IEnumerable<DiscountResponse>>> GetAllDiscounts()
        {
            var discounts = await _discountService.GetAllDiscountsAsync();
            return Ok(discounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiscountResponse>> GetDiscountById(Guid id)
        {
            var discount = await _discountService.GetDiscountByIdAsync(id);
            return Ok(discount);
        }

        [HttpPost]
        public async Task<ActionResult> AddDiscount([FromBody] DiscountResponse discountDto)
        {
            await _discountService.AddDiscountAsync(discountDto);
            return CreatedAtAction(nameof(GetDiscountById), new { id = discountDto.Id }, discountDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateDiscount(Guid id, [FromBody] DiscountResponse discountDto)
        {
            if (id != discountDto.Id)
                return BadRequest("Discount ID mismatch");

            await _discountService.UpdateDiscountAsync(discountDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDiscount(Guid id)
        {
            await _discountService.DeleteDiscountAsync(id);
            return NoContent();
        }
    }
}
