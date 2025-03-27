using DACN_VILLA.Model;

namespace DACN_VILLA.Interface
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateToken(User user);
    }
}
