using DACN_VILLA.DTO.Response;

namespace DACN_VILLA.DTO.Respone
{
    public class VillaListResponse
    {
        public List<VillaResponse> Villas {  get; set; }
        public int TotalPages {  get; set; }
    }
}
