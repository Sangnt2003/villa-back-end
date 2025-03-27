using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using DACN_VILLA.Service;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace DACN_VILLA.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<UserListResponse>> GetAllUsers(int pageNumber = 1, int pageSize = 8)
        {
            var userResponses = await _userService.GetAllUsersAsync();
            int totalUsers = userResponses.Count();


            var pagedUsers = userResponses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new UserListResponse
            {
                Users = pagedUsers,
                TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize) // Calculate total pages
            };

            return Ok(response);
        }
        [HttpPost]
        public async Task<ActionResult<UserResponse>> CreateUser([FromBody] UserCreateRequest dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetUserById(Guid id) // Thay đổi int thành Guid
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser([FromBody] UserUpdateRequest dto)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
            Console.Write(token);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token không hợp lệ hoặc không có token.");
            }

            try
            {
                var userId = _userService.GetUserIdFromToken(token);
                await _userService.UpdateAsync(userId, dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating user: " + ex.Message);
                return StatusCode(500, "Lỗi máy chủ");
            }

            return NoContent();
        }

        [HttpPut("update-password/{id}")]
        public async Task<ActionResult> UpdatePassword([FromBody] PasswordUpdateRequest request)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();
            Console.Write(token);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token không hợp lệ hoặc không có token.");
            }

            try
            {
                var userId = _userService.GetUserIdFromToken(token);
                await _userService.UpdatePasswordAsync(userId, request);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Không tìm thấy User");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi cập nhật: " + ex.Message);
                return StatusCode(500, "Lỗi máy chủ");
            }

            return NoContent();
        }
        /*[HttpPut("role/{userId}")]
        public async Task<ActionResult> ChangeUserRole(string userId, [FromBody] string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isSuperAdmin = await _userManager.IsInRoleAsync(user, "SuperAdmin");
            if (!isSuperAdmin)
            {
                return Unauthorized("Only SuperAdmin can change roles.");
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Nếu người dùng hiện tại là Admin và bạn muốn hạ xuống Customer
            if (currentRoles.Contains("Admin") && newRole == "Customer")
            {
                var resultRemoveAdmin = await _userManager.RemoveFromRoleAsync(user, "Admin");
                if (resultRemoveAdmin.Succeeded)
                {
                    var resultAddCustomer = await _userManager.AddToRoleAsync(user, "Customer");
                    if (resultAddCustomer.Succeeded)
                    {
                        return Ok("User role changed from Admin to Customer.");
                    }
                }
            }

            // Nếu người dùng hiện tại là Customer và bạn muốn nâng lên Admin
            if (currentRoles.Contains("Customer") && newRole == "Admin")
            {
                var resultRemoveCustomer = await _userManager.RemoveFromRoleAsync(user, "Customer");
                if (resultRemoveCustomer.Succeeded)
                {
                    var resultAddAdmin = await _userManager.AddToRoleAsync(user, "Admin");
                    if (resultAddAdmin.Succeeded)
                    {
                        return Ok("User role changed from Customer to Admin.");
                    }
                }
            }

            return BadRequest("Invalid role change request.");
        }*/
    }
}
