using DACN_VILLA.Interface.Service;
using DACN_VILLA.Helper;
using DACN_VILLA.DTO.Request;
using Microsoft.AspNetCore.Identity;
using DACN_VILLA.Model;
using System.Web;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.Data;

namespace DACN_VILLA.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;
        private readonly IUserService _userService;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly UserManager<User> _userManager;

        public AccountController(IAuthService authService, ILogger<AccountController> logger, IUserService userService, IConfiguration configuration, IEmailService emailService, UserManager<User> userManager)
        {
            _authService = authService;
            _logger = logger;
            _userService = userService;
            _userManager = userManager;
            _emailService = emailService;
            _config = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromForm] UserCreateRequest dto, IFormFile? image)
        {
            // Kiểm tra nếu không có hình ảnh được gửi lên
            if (image != null)
            {
                var validFileTypes = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(image.FileName);
                if (!validFileTypes.Contains(fileExtension.ToLower()))
                {
                    return BadRequest(new { PictureUrl = "Chỉ chấp nhận các định dạng hình ảnh (.jpg, .jpeg, .png)." });
                }

                var uploadPath = Path.Combine("uploads", "user");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Tạo tên file ngẫu nhiên
                var fileName = Guid.NewGuid() + fileExtension;
                var filePath = Path.Combine(uploadPath, fileName);

                try
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    dto.PictureUrl = fileName;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi");
                    return StatusCode(500, new { Message = "Đăng ký thất bại." });
                }
            }
            else
            {
                // Nếu không có hình ảnh, không làm gì cả và chỉ để lại PictureUrl là null
                dto.PictureUrl = null;
            }

            // Tiến hành đăng ký người dùng
            var token = await _authService.RegisterAsync(dto);
            return Ok(new { Token = token });
        }

        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmailAsync([FromQuery] string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);

                // Trả về kết quả kiểm tra email
                return Ok(new { emailExists = user != null });
            }
            catch (Exception ex)
            {
                // Ghi log nếu cần thiết
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi kiểm tra email." });
            }
        }


        [HttpGet("check-username")]
        public async Task<IActionResult> CheckUsernameAsync([FromQuery] string username)
        {
            var checkUsername = await _userService.GetByUserAsync(username);
            if (checkUsername != null)
            {
                return Ok(new { usernameExists = true });
            }
            return Ok(new { usernameExists = false });
        }

        [HttpGet("uploads/{*filePath}")]
        public IActionResult GetImage(string filePath)
        {
            // Xây dựng đường dẫn đầy đủ một cách an toàn
            var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            var fullPath = Path.Combine(uploadsDirectory, filePath);

            // Kiểm tra để đảm bảo đường dẫn không trỏ ra ngoài thư mục uploads
            if (!fullPath.StartsWith(uploadsDirectory))
            {
                return BadRequest("Invalid file path."); // Ngăn chặn truy cập trái phép
            }

            // Kiểm tra file có tồn tại hay không
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound(); // Trả về lỗi 404 nếu không tìm thấy ảnh
            }

            // Lấy phần mở rộng và xác định MIME type
            var fileExtension = Path.GetExtension(filePath).ToLower();
            string mimeType = fileExtension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            // Đọc file và trả về
            var fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, mimeType);
        }



        [HttpPost("token")]
        public async Task<IActionResult> Login(AuthenticationRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest(new { Message = "Tài khoản và mật khẩu là bắt buộc." });
            }

            try
            {
                var token = await _authService.LoginAsync(request);
                if (token == null)
                {
                    return Unauthorized(new { Message = "Invalid credentials" });
                }
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đăng nhập");
                return StatusCode(500, "Lỗi server");
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> AuthenticateGoogleToken([FromBody] GoogleTokenRequest request)
        {
            try
            {
                var authResponse = await _authService.AuthenticateGoogleTokenAsync(request);
                return Ok(authResponse);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Google authentication error");
                return StatusCode(500, $"Lỗi máy chủ: {ex.Message}");
            }
        }

        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUserInfo(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new { Message = "Invalid ID" });
            }

            try
            {
                var userResponse = await _userService.GetByIdAsync(id);
                if (userResponse == null)
                {
                    return NotFound(new { Message = "Không tìm thấy người dùng" });
                }

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin người dùng");
                return StatusCode(500, "Lỗi server");
            }
        }



        [HttpPost("request-reset-code")]
        public async Task<IActionResult> RequestResetCode([FromBody] DTO.Request.ForgotPasswordRequest request, [FromServices] IMemoryCache cache)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { Message = "Email là bắt buộc." });
            }

            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound(new { Message = "Không tìm thấy người dùng với email này." });
            }

            var resetCode = new Random().Next(100000, 999999).ToString();

            cache.Set(request.Email, resetCode, TimeSpan.FromMinutes(10));

            await _emailService.SendEmailAsync(user.Email, "Mã Xác Thực Đặt Lại Mật Khẩu",
                $"Mã xác thực của bạn là: {resetCode}. Mã có hiệu lực trong 10 phút.");

            return Ok(new { Message = "Mã xác thực đã được gửi qua email." });
        }

        [HttpPost("verify-reset-code")]
        public async Task<IActionResult> VerifyResetCode([FromBody] VerifyResetCodeRequest request, [FromServices] IMemoryCache cache)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new { Message = "Email và mã xác thực là bắt buộc." });
            }

            if (!cache.TryGetValue(request.Email, out string cachedCode))
            {
                return BadRequest(new { Message = "Mã xác thực không tồn tại hoặc đã hết hạn." });
            }

            if (cachedCode != request.Code)
            {
                return BadRequest(new { Message = "Mã xác thực không hợp lệ." });
            }

            cache.Remove(request.Email);

            return Ok(new { Message = "Xác thực mã thành công." });
        }

        [HttpPost("generate-new-password")]
        public async Task<IActionResult> GenerateNewPassword([FromBody] GenerateNewPasswordRequest request)
        {
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { Message = "Email là bắt buộc." });
            }

            var user = await _userService.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound(new { Message = "Không tìm thấy người dùng với email này." });
            }
            var newPassword = GenerateRandomPassword();

            var resetResult = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), newPassword);
            if (!resetResult.Succeeded)
            {
                return BadRequest(new { Message = "Đặt lại mật khẩu thất bại." });
            }

            await _emailService.SendEmailAsync(user.Email, "Mật Khẩu Mới", $"Mật khẩu mới của bạn là: {newPassword}");

            return Ok(new { Message = "Mật khẩu mới đã được gửi qua email." });
        }

        private string GenerateRandomPassword()
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "@#$%";

            var random = new Random();
            var password = new StringBuilder();

            password.Append(upperChars[random.Next(upperChars.Length)]);
            password.Append(lowerChars[random.Next(lowerChars.Length)]);
            password.Append(specialChars[random.Next(specialChars.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(digits[random.Next(digits.Length)]);

            return password.ToString();
        }
    }
}
