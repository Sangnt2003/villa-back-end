namespace DACN_VILLA.DTO.Request
{
    public class VillaSearchRequest
    {
        public Guid LocationId { get; set; }
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; } 
        public int? Capacity { get; set; }      
    }
}
