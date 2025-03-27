using System.ComponentModel.DataAnnotations;

namespace DACN_VILLA.Model
{
    public class Review
    {
        public Guid Id { get; set; }
        public Guid VillaId { get; set; }
        public Guid CustomerId { get; set; }
        [Range(1, 5)]
        public int Rating { get; set; }
        public int? PreCmtId { get; set; }
        [StringLength(1000)]
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Villa Villa { get; set; }
        public User Customer { get; set; }
    }
}
