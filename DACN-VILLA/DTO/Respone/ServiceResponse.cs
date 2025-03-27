namespace DACN_VILLA.DTO.Respone
{
    public class ServiceResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } // Price per service
    }

}
