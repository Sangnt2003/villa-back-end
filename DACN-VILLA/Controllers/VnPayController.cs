using Microsoft.AspNetCore.Mvc;
using SH_SHOP.VNPay;
using System.Globalization;
using static SH_SHOP.VNPay.VnPayService;

namespace DACN_VILLA.Controllers
{
    [ApiController]
    [Route("api/vnpay")]
    public class VnPayController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        public VnPayController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        [HttpPost("create-payment")]
        public IActionResult CreatePayment([FromBody] VnPaymentRequestModel request)
        {
            var paymentUrl = _vnPayService.CreatePayment(request);
            return Ok(new { paymentUrl });
        }

        [HttpGet("payment-callback")]
        public IActionResult PaymentCallback()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.Success)
            {
                // Redirect về trang payment-callback trên frontend
                return Redirect($"http://localhost:5173/payment-callback?success=true&paymentMethod={response.PaymentMethod}&orderDescription={Uri.EscapeDataString(response.OrderDescription)}&orderId={response.OrderId}&transactionId={response.TransactionId}&totalAmount={response.TotalAmount}&villaOwnerAmount={response.VillaOwnerAmount}&platformFee={response.PlatformFee}");
            }
            else
            {
                // Redirect về trang payment-callback với thông báo thất bại
                return Redirect($"http://localhost:5173/payment-callback?success=false&message={Uri.EscapeDataString(response.OrderDescription)}");
            }
        }


    }
}
