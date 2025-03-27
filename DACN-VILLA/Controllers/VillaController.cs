using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.DTO.Response;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model.Enum;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing.Printing;

namespace DACN_VILLA.Controllers
{
    [ApiController]
    [Route("api/villa")]
    public class VillaController : ControllerBase
    {
        private readonly IVillaService _villaService;
        private readonly ILogger<VillaController> _logger;
        private readonly ApplicationDbContext _context;

        public VillaController(IVillaService villaService, ILogger<VillaController> logger, ApplicationDbContext context)
        {
            _villaService = villaService;
            _logger = logger;
            _context = context;
        }

        [HttpGet("villas")]
        public async Task<ActionResult<VillaListResponse>> GetAllVillas(int pageNumber = 1, int pageSize = 8)
        {
            var villaResponses = await _villaService.GetAllVillasAsync();
            int totalVillas = villaResponses.Count();


            var pagedVillas = villaResponses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new VillaListResponse
            {
                Villas = pagedVillas,
                TotalPages = (int)Math.Ceiling((double)totalVillas / pageSize) // Calculate total pages
            };

            return Ok(response);
        }
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<VillaResponse>>> GetAll()
        {
            var villas = await _villaService.GetApprovedVillasAsync();
            return Ok(villas);
        }

        [HttpGet("villas/{userId}")]
        public async Task<ActionResult<VillaListResponse>> GetVillasByUserId(Guid userId, int pageNumber = 1, int pageSize = 3)
        {
            var villaResponses = await _villaService.GetAllVillaByUserIdAsync(userId);

            int totalVillas = villaResponses.Count();

            var pagedVillas = villaResponses
                .Skip((pageNumber - 1) * pageSize)  // Bỏ qua các biệt thự trước đó
                .Take(pageSize)                      // Lấy số lượng biệt thự theo trang
                .ToList();                            // Chuyển kết quả thành danh sách

            // Tạo đối tượng trả về với thông tin biệt thự đã phân trang
            var response = new VillaListResponse
            {
                Villas = pagedVillas,
                TotalPages = (int)Math.Ceiling((double)totalVillas / pageSize) // Tính tổng số trang
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VillaResponse>> GetVillaById(Guid id)
        {
            var villa = await _villaService.GetVillaByIdAsync(id);
            if (villa == null)
                return NotFound($"Villa with ID {id} not found.");

            return Ok(villa);
        }

        [HttpPost]
        public async Task<ActionResult> AddVilla([FromForm] VillaCreateRequest villaCreateRequest, [FromForm] IFormFile[] image)
        {
            if (villaCreateRequest == null)
                return BadRequest("Villa data is required.");

            // Kiểm tra nếu có tệp ảnh
            if (image != null && image.Length > 0)
            {
                var uploadPath = Path.Combine("uploads", "villa");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var imageUrls = new List<string>();

                // Xử lý từng tệp hình ảnh
                foreach (var file in image)
                {
                    var validFileTypes = new[] { ".jpg", ".jpeg", ".png" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (!validFileTypes.Contains(fileExtension))
                    {
                        return BadRequest(new { PictureUrl = "Chỉ chấp nhận các định dạng hình ảnh (.jpg, .jpeg, .png)." });
                    }

                    var fileName = Guid.NewGuid() + fileExtension;
                    var filePath = Path.Combine(uploadPath, fileName);

                    try
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        imageUrls.Add(fileName); // Thêm URL hình ảnh vào danh sách
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi lưu hình ảnh.");
                        return StatusCode(500, new { Message = "Lỗi khi lưu hình ảnh." });
                    }
                }

                villaCreateRequest.ImageUrls = imageUrls; // Gán các URL hình ảnh vào villaCreateRequest
            }
            else
            {
                villaCreateRequest.ImageUrls = new List<string>(); // Nếu không có ảnh, gán danh sách trống
            }

            // Lưu villa và ảnh vào cơ sở dữ liệu
            var villaResponse = await _villaService.AddVillaAsync(villaCreateRequest);

            return CreatedAtAction(nameof(GetVillaById), new { id = villaResponse.Id }, villaResponse);
        }


        [HttpPut("{id}/approve")]
        public async Task<ActionResult> ApproveVilla(Guid id, [FromBody] ApprovalRequest approvalRequest)
        {
            Console.WriteLine($"Received Status: {approvalRequest.Status}"); // Debug giá trị nhận được
            if (approvalRequest == null)
                return BadRequest("Approval request is required.");

            var result = await _villaService.ApproveVillaAsync(id, (ApprovalStatus)approvalRequest.Status); // Cast nếu cần
            if (!result)
                return NotFound("Villa not found or invalid status.");

            return Ok("Villa status updated successfully.");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateVilla(Guid id, [FromBody] VillaUpdateRequest villaUpdateRequest)
        {
            if (villaUpdateRequest == null)
                return BadRequest("Villa data is required.");

            var existingVilla = await _villaService.GetVillaByIdAsync(id);
            if (existingVilla == null)
                return NotFound($"Villa with ID {id} not found.");

            villaUpdateRequest.Id = id;

            await _villaService.UpdateVillaAsync(villaUpdateRequest);

            return Ok("Cập nhật thành công");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVilla(Guid id)
        {
            var existingVilla = await _villaService.GetVillaByIdAsync(id);
            if (existingVilla == null)
                return NotFound($"Villa with ID {id} not found.");

            await _villaService.DeleteVillaAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchVillas([FromQuery] VillaSearchRequest request)
        {
            if (request == null || request.LocationId == Guid.Empty || request.StartDate == default || request.EndDate == default || request.Capacity <= 0)
            {
                return BadRequest("Thông tin tìm kiếm không hợp lệ.");
            }

            var availableVillas = await _villaService.SearchAvailableVillas(request);
            return Ok(availableVillas);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterVillas([FromQuery] VillaFilterRequest request)
        {
            var availableVillas = await _villaService.FilterVillas(request);
            return Ok(availableVillas);
        }

        [HttpGet("nearby/{userId}")]
        public async Task<IActionResult> GetVillasNearbyUser(Guid userId)
        {
            try
            {
                var villas = await _villaService.GetVillasNearbyUserAsync(userId);
                return Ok(villas);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}