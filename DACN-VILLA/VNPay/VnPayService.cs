
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using static DACN_VILLA.Controllers.VnPayController;

namespace SH_SHOP.VNPay
{
    public class VnPayService : IVnPayService
    {
        public readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly IVillaRepository _villaRepository;
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());
        public VnPayService(IConfiguration config, IBookingRepository bookingRepository, IUserRepository userRepository, IVillaRepository villaRepository)
        {
            _config = config;
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _villaRepository = villaRepository;
        }
        public string CreatePayment( VnPaymentRequestModel request)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng " + request.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);
            var expireDate = DateTime.UtcNow.AddMinutes(1000).ToString("yyyyMMddHHmmss");
            vnpay.AddRequestData("vnp_ExpireDate", expireDate);

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
            return paymentUrl;
        }
        public class VnPayCompare : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x == y) return 0;
                if (x == null) return -1;
                if (y == null) return 1;
                var vnpCompare = CompareInfo.GetCompareInfo("en-US");
                return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
            }
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collection)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collection)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = vnpay.GetResponseData("vnp_TxnRef");
            var vnp_TransactionId = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = collection.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            var amountString = vnpay.GetResponseData("vnp_Amount");

            decimal amount = decimal.Parse(amountString) / 100;

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    OrderDescription = "Chữ ký không hợp lệ."
                };
            }
            if (vnp_ResponseCode != "00")
            {
                return new VnPaymentResponseModel
                {
                    Success = false,
                    OrderDescription = $"Thanh toán thất bại. Mã phản hồi: {vnp_ResponseCode}",
                };
            }
            var userId = new Guid("6d05b786-8721-4e5a-5d6b-08dd0220a5c2");
            string uuid = vnp_OrderInfo.Split(' ').LastOrDefault();

            Guid villaOwnerId = Guid.Empty;

            if (Guid.TryParse(uuid, out Guid bookingId))
            {
                _bookingRepository.UpdateBookingStatusAndBalanceAsync(bookingId, userId, amount).Wait();

                // Lấy VillaId từ BookingId
                var villaId = _bookingRepository.GetVillaIdByBookingId(bookingId);
                if (villaId != null)
                {
                    villaOwnerId = _villaRepository.GetOwnerIdByVillaId(villaId);
                }
            }
            else
            {
                throw new ArgumentException("uuid không phải là một GUID hợp lệ.");
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "vnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId,
                TransactionId = vnp_TransactionId,
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
                VillaOwnerId = villaOwnerId, // Gán UserId của chủ Villa
            };
        }
    }
}
