using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Model;
using Microsoft.AspNetCore.Identity;

namespace DACN_VILLA.Interface.Service
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(UserCreateRequest userDto);
        Task<DuplicateCheckResponse> CheckDuplicateAsync(string username, string email);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUserAsync(string username);
        Task<UserResponse> GetByIdAsync(Guid id);
        Task<UserResponse> RegisterUserAsync(UserCreateRequest user, string password);
        Task UpdateAsync(Guid userId, UserUpdateRequest request);
        Task UpdatePasswordAsync(Guid userId, PasswordUpdateRequest request);
        Task AddAsync(UserCreateRequest newUser);
        Guid GetUserIdFromToken(string token);
    }

}
