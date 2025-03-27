using DACN_VILLA.DTO;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Helper;
using DACN_VILLA.Interface;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace DACN_VILLA.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IJwtProvider _jwtProvider;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IHttpClientFactory httpClientFactory,
            IJwtProvider jwtProvider,
            IPasswordHasher<User> passwordHasher,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _httpClientFactory = httpClientFactory;
            _jwtProvider = jwtProvider;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<AuthResponseDto> AuthenticateGoogleTokenAsync(GoogleTokenRequest request)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(request.Token) as JwtSecurityToken;

            if (jsonToken == null)
            {
                throw new UnauthorizedAccessException("Token is not valid.");
            }

            var email = jsonToken?.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = jsonToken?.Claims.FirstOrDefault(c => c.Type == "name")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
            {
                throw new UnauthorizedAccessException("Token payload is invalid.");
            }

            var user = await _userRepository.GetByEmailAsync(email);
            UserCreateRequest userCreateRequest = new UserCreateRequest
            {
                Email = email,
                FullName = name,
                UserName = email,
                Address = "Rỗng",
                PictureUrl = "https://lh3.googleusercontent.com/a/ACg8ocIgztPqgX9icqlUyIEuQAkDZvCSb94H7OBbk3lOwsXI5jUDSzOz=s96-c",
                PhoneNumber = "",
                Role = Role.Role_Customer
            };

            if (user == null)
            {
                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    UserName = userCreateRequest.UserName,
                    FullName = userCreateRequest.FullName,
                    Email = userCreateRequest.Email,
                    PictureUrl = userCreateRequest.PictureUrl,
                    Address = userCreateRequest.Address,
                    PhoneNumber = userCreateRequest.PhoneNumber,
                };

                await _userRepository.AddAsync(newUser);

                var role = await _roleManager.FindByNameAsync(Role.Role_Customer);
                if (role == null)
                {
                    role = new IdentityRole<Guid>(Role.Role_Customer);
                    await _roleManager.CreateAsync(role);
                }

                await _userManager.AddToRoleAsync(newUser, Role.Role_Customer);
            }
            else
            {
                user.FullName = userCreateRequest.FullName;
                user.PictureUrl = userCreateRequest.PictureUrl;
                user.Address = userCreateRequest.Address;
                user.PhoneNumber = userCreateRequest.PhoneNumber;

                await _userRepository.UpdateAsync(user);
            }

            string jwt = _jwtProvider.GenerateToken(new ClaimsPrincipal(
                new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, email) })));

            return new AuthResponseDto
            {
                IdentityToken = jwt,
                User = userCreateRequest
            };
        }

        public async Task<string> RegisterAsync(UserCreateRequest request)
        {
            var existingUserByUsername = await _userRepository.GetByUserName(request.UserName);
            if (existingUserByUsername != null)
            {
                throw new ArgumentException("Tài khoản đã được đăng ký");
            }

            var existingUserByEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUserByEmail != null)
            {
                throw new ArgumentException("Email đã được đăng ký");
            }

            var newUser = new User
            {
                UserName = request.UserName,
                FullName = request.FullName,
                Email = request.Email,
                Address = request.Address,
                PictureUrl = request.PictureUrl,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(newUser, request.PasswordHash);
            if (!result.Succeeded)
            {
                throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            var role = await _roleManager.FindByNameAsync(Role.Role_Customer);
            if (role == null)
            {
                role = new IdentityRole<Guid>(Role.Role_Customer);
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(newUser, Role.Role_Customer);

            var token = await _jwtTokenGenerator.GenerateToken(newUser);
            return token;
        }

        public async Task<string> LoginAsync(AuthenticationRequest loginDto)
        {
            var user = await _userRepository.GetByUserName(loginDto.Username);
            if (user == null || !await _userRepository.VerifyPasswordAsync(user, loginDto.Password))
            {
                throw new UnauthorizedAccessException("Tài khoản hoặc mật khẩu không hợp lệ");
            }

            if (user.Id == Guid.Empty)
            {
                throw new Exception("ID người dùng không hợp lệ.");
            }

            var token = await _jwtTokenGenerator.GenerateToken(user);
            return token;
        }
    }
}
