namespace DACN_VILLA.Model
{
    public class Location
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<Villa> Villas { get; set; }
    }
}
