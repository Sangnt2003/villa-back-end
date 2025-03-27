namespace SH_SHOP.VNPay
{
    public interface IVnPayService
    {
        string CreatePayment(VnPaymentRequestModel request);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collection);
    }
}
