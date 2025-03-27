namespace DACN_VILLA.DTO.Request
{
    public class WishlistCreateRequest
    {
        public Guid UserId { get; set; }
        public Guid VillaId {  get; set; }
        public DateTime CreateAt { get; set; }
    }
}
