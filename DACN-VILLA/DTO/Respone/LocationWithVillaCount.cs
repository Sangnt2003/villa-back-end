namespace DACN_VILLA.DTO.Respone
{
    public class LocationWithVillaCount
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int TotalBooking {  get; set; }
        public int TotalVilla { get; set; }
    }

}
