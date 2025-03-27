using System.ComponentModel.DataAnnotations;

namespace DACN_VILLA.DTO.Request
{
    public class ReviewRequest
    {
        public Guid VillaId { get; set; } 
        public Guid UserId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string Comment { get; set; } 

        public int? PreCmtId { get; set; } 
    }

}
