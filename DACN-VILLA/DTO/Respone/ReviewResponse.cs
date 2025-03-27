namespace DACN_VILLA.DTO.Respone
{
    public class ReviewResponse
    {
        public Guid Id { get; set; }
        public Guid VillaId { get; set; }
        public Guid UserId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}
