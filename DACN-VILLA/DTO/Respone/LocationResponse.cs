using DACN_VILLA.DTO.Response;

namespace DACN_VILLA.DTO.Respone
{
    public class LocationResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalVilla {  get; set; }
        public string ImageUrl { get; set; }
        public List<VillaResponse> Villas { get; set; }
    }
}
