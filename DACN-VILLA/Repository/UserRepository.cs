using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace DACN_VILLA.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<UserRepository> _logger;
        private readonly UserManager<User> _userManager;

        public UserRepository(ApplicationDbContext context,UserManager<User> userManager, IPasswordHasher<User> passwordHasher, ILogger<UserRepository> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _userManager = userManager; 
        }



        public async Task UpdateAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }


        public async Task AddAsync(User newUser)
        {
            try
            {
                await _context.Users.AddAsync(newUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error occurred while saving user to database.");
                throw; // Re-throw the exception after logging
            }
        }

        public async Task<User> GetByUserNameOrEmailAsync(string userNameOrEmail)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userNameOrEmail || u.Email == userNameOrEmail);
        }

        public async Task<User> RegisterUserAsync(User user, string password)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            await AddAsync(user); // Reusing the AddAsync method
            return user;
        }

        // Example usage of the above method
        public async Task<IEnumerable<User>> GetUsersWithRolesAsync()
        {
            var users = await _context.Users.ToListAsync();
            foreach (var user in users)
            {
                var roles = await GetUserRolesAsync(user);
                // Add roles information to the user or return them as needed
            }
            return users;
        }
        public async Task<bool> VerifyPasswordAsync(User user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Success;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<User> GetByUserName(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}
