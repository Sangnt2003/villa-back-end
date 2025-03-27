using System.ComponentModel.DataAnnotations;

namespace DACN_VILLA.DTO.Request
{
    public class AuthenticationRequest
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
