using System;
using System.Collections.Generic;

namespace DACN_VILLA.DTO.Response
{
    public class WishlistResponse
    {
        public Guid Id { get; set; } 
        public Guid UserId { get; set; } 
        public Guid VillaId { get; set; } 
        public string Name { get; set; } 
        public string Description { get; set; }
        public decimal PricePerNight { get; set; } 
        public decimal ListedPrice { get; set; }
        public int Rating { get; set; }
        public List<string> ImageUrls { get; set; } 
        public int Capacity { get; set; } 
        public string Address { get; set; } 
        public List<string> VillaServices { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
