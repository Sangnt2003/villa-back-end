using System;
using DACN_VILLA.Model;

namespace DACN_VILLA.Model
{
    public class Wishlist
    {
        public Guid Id { get; set; } = Guid.NewGuid(); 
        public Guid UserId { get; set; } 
        public User User { get; set; } 
        public Guid VillaId { get; set; } 
        public Villa Villa { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
