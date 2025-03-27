using System.ComponentModel.DataAnnotations;

namespace DACN_VILLA.DTO
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? PictureUrl { get; set;}
    }
}
