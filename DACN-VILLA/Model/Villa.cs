using DACN_VILLA.Model;
using DACN_VILLA.Model.Enum;
using System.Text.Json.Serialization;

public class Villa
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LocationId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string PhoneNumber { get; set; }
    public string Description { get; set; }
    public decimal PricePerNight { get; set; }
    public decimal ListedPrice { get; set; }
    public int Capacity { get; set; }
    public int Rating { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public User User { get; set; }
    public Location Location { get; set; }
    public ICollection<VillaImage> VillaImages { get; set; } 
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    [JsonIgnore]
    public ICollection<VillaServices> VillaServices { get; set; }
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
