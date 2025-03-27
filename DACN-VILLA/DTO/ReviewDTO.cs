namespace DACN_VILLA.DTO
{
    public class ReviewDTO
    {
        public Guid Id { get; set; }
        public Guid VillaId { get; set; }
        public Guid CustomerId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
    }

}
