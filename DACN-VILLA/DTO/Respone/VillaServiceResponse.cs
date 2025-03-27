namespace DACN_VILLA.DTO.Respone
{
    public class VillaServiceResponse
    {
        public Guid VillaId { get; set; }
        public Guid ServiceId { get; set; }
        public bool IsAvailable { get; set; }
    }
}
