namespace DACN_VILLA.DTO
{
    public class BookingProcessDTO
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Status { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string Note { get; set; }
    }

}
