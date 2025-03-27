using Microsoft.AspNetCore.Identity;

namespace DACN_VILLA.Model
{
    public class User : IdentityUser<Guid> 
    {
        public string FullName { get; set; }
        public string? PictureUrl { get; set; }
        public string Address { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
    }
}
