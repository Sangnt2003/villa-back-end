using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model.Enum;
using DACN_VILLA.Service;
using Microsoft.AspNetCore.Mvc;
using SH_SHOP.VNPay;
using System;
using System.Threading.Tasks;

namespace DACN_VILLA.Controllers
{
    [Route("api/booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IVnPayService _vnPayService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService, IVnPayService vnPayService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _vnPayService = vnPayService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<ActionResult<BookingListResponse>> GetAllBookings(int pageNumber = 1, int pageSize = 6)
        {
            var bookings = await _bookingService.GetAllBookingAsync();
            var pagedBookings = bookings
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new BookingListResponse
            {
                Bookings = pagedBookings,
                TotalPages = (int)Math.Ceiling((double)bookings.Count() / pageSize)
            };

            return Ok(response);
        }

        [HttpPost("create-booking")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
        {
            try
            {
                var booking = await _bookingService.CreateBookingAsync(request);
                var paymentRequest = new VnPaymentRequestModel
                {
                    Amount = request.TotalPrice,
                    OrderId = booking.BookingId,
                };

                var paymentUrl = _vnPayService.CreatePayment(paymentRequest);

                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Có lỗi xảy ra: " + ex.Message);
            }    
        }

        [HttpGet("payment-callback")]
        public async Task<IActionResult> PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.Success)
            {
                return Redirect($"http://localhost:5173/payment-callback?success=true" +
                                $"&paymentMethod={response.PaymentMethod}" +
                                $"&orderDescription={Uri.EscapeDataString(response.OrderDescription)}" +
                                $"&orderId={response.OrderId}" +
                                $"&transactionId={response.TransactionId}" +
                                $"&totalAmount={response.TotalAmount}" +
                                $"&villaOwnerAmount={response.VillaOwnerAmount}" +
                                $"&platformFee={response.PlatformFee}" +
                                $"&villaOwnerId={response.VillaOwnerId}");
            }
            else
            {
                return Redirect($"http://localhost:5173/payment-callback?success=false" +
                                $"&message={Uri.EscapeDataString(response.OrderDescription)}");
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [HttpGet("bookings/{userId}")]
        public async Task<ActionResult<BookingListResponse>> GetBookingsByUserId(Guid userId, int pageNumber = 1, int pageSize = 3)
        {
            var bookingResponses = await _bookingService.GetBookingByUserIdAsync(userId);

            int totalBookings = bookingResponses.Count();

            var pagedBookings = bookingResponses
                .Skip((pageNumber - 1) * pageSize)  
                .Take(pageSize)                   
                .ToList();                        

            var response = new BookingListResponse
            {
                Bookings = pagedBookings,
                TotalPages = (int)Math.Ceiling((double)totalBookings / pageSize)
            };
            return Ok(response);
        }

        [HttpPost("cancel-booking/{id}")]
        public async Task<IActionResult> CancelBooking(Guid id)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(id);
                if (!result)
                {
                    return BadRequest("Booking cancellation failed.");
                }

                return Ok("Booking cancelled and refund processed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking.");
                return StatusCode(500, "An error occurred while processing the cancellation.");
            }
        }
        [HttpPost("refund-to-owner")]
        public async Task<IActionResult> RefundToOwner()
        {
            try
            {
                // Gọi phương thức hoàn tiền từ dịch vụ
                var result = await _bookingService.RefundToOwnerAsync();
                if (result)
                {
                    return Ok("Refund processed successfully.");
                }
                else
                {
                    return BadRequest("No bookings eligible for refund.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund to owner.");
                return StatusCode(500, "An error occurred while processing the refund.");
            }
        }

    }
}
