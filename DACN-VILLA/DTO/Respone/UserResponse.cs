using System.Runtime.CompilerServices;

namespace DACN_VILLA.DTO.Respone
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PictureUrl { get; set; }
        public string Address {  get; set; }
        public string PhoneNumber {  get; set; }
        public decimal Balance {  get; set; }
        public IEnumerable<string> Roles { get; set; } // Add roles property
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
