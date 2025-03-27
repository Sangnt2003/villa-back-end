namespace DACN_VILLA.DTO.Request
{
    public class VillaUpdateRequest
    {
        public Guid Id { get; set; } 
        public Guid LocationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public decimal? PricePerNight { get; set; }
        public int? Capacity { get; set; }
        public int? Rating { get; set; }
        public bool? Status { get; set; }
        public List<string> ImageUrls { get; set; } 
    }
}
