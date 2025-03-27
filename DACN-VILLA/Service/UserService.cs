using AutoMapper;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using NuGet.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace DACN_VILLA.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserService> _logger;
        public UserService(IUserRepository userRepository,ApplicationDbContext context, ILogger<UserService> logger, IMapper mapper, IPasswordHasher<User> passwordHasher, UserManager<User> userManager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _context = context;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task AddAsync(UserCreateRequest newUser)
        {
            var userEntity = _mapper.Map<User>(newUser);
            await _userRepository.AddAsync(userEntity);
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            // Get all users from the repository
            var users = await _userRepository.GetAllUsersAsync();

            // Create a list to hold the mapped UserResponse objects
            var userResponses = new List<UserResponse>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userResponse = _mapper.Map<UserResponse>(user);
                userResponse.Roles = roles;

                userResponses.Add(userResponse);
            }

            return userResponses;
        }

        public async Task<User> GetByUserAsync(string username)
        {
            return await _userRepository.GetByUserName(username);
           
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return  await _userRepository.GetByEmailAsync(email);
           
        }
        public async Task<DuplicateCheckResponse> CheckDuplicateAsync(string username, string email)
        {
            bool isUsernameTaken = await _context.Users.AnyAsync(u => u.UserName == username);

            bool isEmailTaken = await _context.Users.AnyAsync(u => u.Email == email);

            // Return the result
            return new DuplicateCheckResponse
            {
                IsUsernameTaken = isUsernameTaken,
                IsEmailTaken = isEmailTaken
            };
        }
        public async Task<UserResponse> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"Không tìm thấy User có ID {id}.");

            var roles = await _userManager.GetRolesAsync(user);

            var userResponse = _mapper.Map<UserResponse>(user);
            userResponse.Roles = roles;

            return userResponse;
        }

        public async Task<UserResponse> CreateUserAsync(UserCreateRequest userDto)
        {
            var existingUsername = await _userRepository.GetByUserName(userDto.UserName);
            var existingEmail = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUsername != null && existingEmail != null)
            {
                throw new Exception("User đã tồn tại");
            }

            var userEntity = _mapper.Map<User>(userDto);
            userEntity.PasswordHash = _passwordHasher.HashPassword(userEntity, userDto.PasswordHash);
            await _userRepository.RegisterUserAsync(userEntity, userDto.PasswordHash);

            return _mapper.Map<UserResponse>(userEntity);
        }

        public async Task UpdateAsync(Guid userId, UserUpdateRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy người dùng");

            // Lấy danh sách các role hiện tại của người dùng
            var currentRoles = await _userManager.GetRolesAsync(user);

            user.FullName = request.FullName ?? user.FullName;
            user.Email = request.Email ?? user.Email;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.Address = request.Address ?? user.Address;
            
            if (request.Role != null && !currentRoles.Contains(request.Role))
            {
                await _userManager.AddToRoleAsync(user, request.Role);
            }
            await _userRepository.UpdateAsync(user);
        }

        public async Task UpdatePasswordAsync(Guid userId, PasswordUpdateRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Không tìm thấy người dùng");
            if (!string.IsNullOrEmpty(request.CurrentPassword) && !string.IsNullOrEmpty(request.NewPassword))
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
                if (result == PasswordVerificationResult.Failed)
                {
                    throw new UnauthorizedAccessException("Mật khẩu hiện tại không chính xác");
                }

                user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            }
            await _userRepository.UpdateAsync(user);
        }
        // Implement RegisterUserAsync method
        public async Task<UserResponse> RegisterUserAsync(UserCreateRequest userDto, string password)
        {
            var existingUsername = await _userRepository.GetByUserName(userDto.UserName);
            var existingEmail = await _userRepository.GetByEmailAsync(userDto.Email);
            if (existingUsername != null && existingEmail != null)
            {
                throw new Exception("User đã tồn tại");
            }

            var userEntity = _mapper.Map<User>(userDto);
            userEntity.PasswordHash = _passwordHasher.HashPassword(userEntity, password);
            await _userRepository.RegisterUserAsync(userEntity, password);

            return _mapper.Map<UserResponse>(userEntity);
        }

        // Implement GetUserIdFromToken method
        public Guid GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:SigningKey"]);

            try
            {
                var principal = tokenHandler.ReadToken(token) as JwtSecurityToken;
                var userIdClaim = principal?.Claims.FirstOrDefault(c => c.Type == "id");

                if (userIdClaim == null)
                {
                    throw new Exception("User ID not found in token.");
                }

                return Guid.Parse(userIdClaim.Value);
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid token.", ex);
            }
        }

     
    }
}
