namespace DACN_VILLA.DTO.Respone
{
    public class DiscountResponse
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
