using DACN_VILLA.Model;

public class Services
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public ICollection<VillaServices> VillaServices { get; set; } 
}
