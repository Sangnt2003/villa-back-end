using DACN_VILLA.DTO.Request;

namespace DACN_VILLA.DTO
{
    public class AuthResponseDto
    {
        public string IdentityToken { get; set; }
        public UserCreateRequest User { get; set; }
    }
}
