using DACN_VILLA.DTO.Response;

namespace DACN_VILLA.DTO.Respone
{
    public class LocationListResponse
    {
        public List<LocationResponse> Locations { get; set; }
        public int TotalPages { get; set; }
    }

}
