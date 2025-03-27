using DACN_VILLA.Model.Enum;

namespace DACN_VILLA.DTO.Request
{
    public class VillaFilterRequest
    {
        public decimal? MinPrice { get; set; } 
        public decimal? MaxPrice { get; set; }
        public int? Capacity { get; set; } 
        public Guid? LocationId { get; set; } 
        public int? Rating { get; set; } 
        public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Approved;
        public bool? IsAvailable { get; set; } 
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
