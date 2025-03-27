using System.Security.Claims;

namespace DACN_VILLA.Interface
{
    public interface IJwtProvider
    {
        string GenerateToken(ClaimsPrincipal principal);
        string GenerateToken(string email, string userId);
    }
}
