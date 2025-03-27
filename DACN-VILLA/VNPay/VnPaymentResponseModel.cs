namespace SH_SHOP.VNPay
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }  // Indicates if payment was successful
        public string PaymentMethod { get; set; }  // Payment method used (e.g., credit card, PayPal, etc.)
        public string OrderDescription { get; set; }  // Description of the order (may include villa details)
        public string OrderId { get; set; }  // The unique order ID related to the booking
        public string PaymentId { get; set; }  // Payment ID provided by VNPay
        public string TransactionId { get; set; }  // Transaction ID for the payment
        public string Token { get; set; }  // A token related to the transaction for security
        public string VnPayResponseCode { get; set; }
        public Guid VillaOwnerId {  get; set; }
        public decimal TotalAmount { get; set; }  // Total amount paid (ListedPrice)
        public decimal VillaOwnerAmount { get; set; }  // Amount to be paid to villa owner (PricePerNight)
        public decimal PlatformFee { get; set; }  // Platform fee (difference between ListedPrice and PricePerNight)
    }

    public class VnPaymentRequestModel
    {
        public Guid OrderId { get; set; } 
        public string FullName { get; set; } 
        public string Description {  get; set; }
        public decimal Amount { get; set; } 
        public string? Address { get; set; } 
        public DateTime CreatedDate { get; set; }
        public decimal VillaOwnerAmount { get; set; }  
        public decimal PlatformFee { get; set; }
    }
}
