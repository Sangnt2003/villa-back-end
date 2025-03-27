using DACN_VILLA.DTO;
using DACN_VILLA.Model;
using Microsoft.AspNetCore.Identity;

namespace DACN_VILLA.Interface.Repository
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUserName(string userName);
        Task<IList<string>> GetUserRolesAsync(User user);
        Task<User> GetByIdAsync(Guid id);
        Task<User> RegisterUserAsync(User user, string password);
        Task<bool> VerifyPasswordAsync(User user, string password);
        Task UpdateAsync(User user);
        Task AddAsync(User newUser);
    }
}
