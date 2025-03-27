using DACN_VILLA.Model.Enum;

namespace DACN_VILLA.DTO.Request
{
    public class VillaCreateRequest
    {
        public Guid LocationId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal ListedPrice { get; set; }
        public int Capacity { get; set; }
        public int Rating { get; set; }
        public List<string> ImageUrls { get; set; }
        public List<Guid> VillaServiceIds { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    }
}
