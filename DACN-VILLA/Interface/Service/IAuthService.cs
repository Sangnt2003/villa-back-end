using DACN_VILLA.DTO;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.Helper;
using Microsoft.AspNetCore.Identity;

namespace DACN_VILLA.Interface.Service
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(UserCreateRequest userDto);
        Task<string> LoginAsync(AuthenticationRequest loginDto);
        Task<AuthResponseDto> AuthenticateGoogleTokenAsync(GoogleTokenRequest request);
    }
}
