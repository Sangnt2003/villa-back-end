namespace DACN_VILLA.Model
{
    public class Discount
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public decimal Percentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<Villa> Villas { get; set; }
    }

}
