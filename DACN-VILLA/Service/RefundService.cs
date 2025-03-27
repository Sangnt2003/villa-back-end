using DACN_VILLA.Interface.Service;

namespace DACN_VILLA.Service
{
    public class RefundService : IRefundService
    {
        private readonly IBookingService _bookingService;

        public RefundService(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task ProcessRefunds()
        {
            await _bookingService.RefundToOwnerAsync();
        }
    }

}
