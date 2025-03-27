using DACN_VILLA.DTO.Respone;

namespace DACN_VILLA.DTO.Response
{
    public class VillaResponse
    {
        public Guid Id { get; set; }
        public Guid LocationId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string LocationName {  get; set; }
        public decimal Percentage {  get; set; }
        public decimal PricePerNight { get; set; }
        public decimal ListedPrice { get; set; }
        public int Capacity { get; set; }
        public int Rating { get; set; }
        public List<string> ImageUrls { get; set; }
        public DateTime AvailableFrom { get; set; }
        public DateTime AvailableTo { get; set; }
        public List<string> VillaServices { get; set; } 
        public string ApprovalStatus { get; set; }
        public List<ReviewResponse> Reviews { get; set; }
    }
}
