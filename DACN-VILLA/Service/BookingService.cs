using AutoMapper;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.DTO.Response;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using DACN_VILLA.Model.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SH_SHOP.VNPay;

namespace DACN_VILLA.Service
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly IVillaRepository _villaRepository;
        private readonly IVnPayService _vnPayService;
        private readonly IMapper _mapper;
        private readonly ILogger<BookingService> _logger;
        public BookingService(ApplicationDbContext context,
            IBookingRepository bookingRepository,
            IMapper mapper,
            IVillaRepository villaRepository,
            IUserRepository userRepository,
            UserManager<User> userManager,
            IVnPayService vnPayService,
            ILogger<BookingService> logger)
        {
            _context = context;
            _villaRepository = villaRepository;
            _mapper = mapper;
            _userManager = userManager;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _vnPayService = vnPayService;
            _logger = logger;   
        }

        public async Task<bool> UpdateBookingStatusAsync(Guid bookingId)
        {
            var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return false;
            booking.ApprovalStatus = (ApprovalStatusBooking)(int)ApprovalStatusBooking.Complete;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BookingResponse> CreateBookingAsync(BookingRequest request)
        {
            var isBooked = await _context.Bookings
                .AnyAsync(b => b.VillaId == request.VillaId &&
                               b.CheckInDate < request.checkOutDate &&
                               b.CheckOutDate > request.checkInDate);
            if (isBooked) throw new InvalidOperationException("Villa đã được đặt trong thời gian này.");

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                VillaId = request.VillaId,
                UserId = request.UserId,
                CheckInDate = request.checkInDate,
                CheckOutDate = request.checkOutDate,
                CheckInHour = request.checkInHour,
                TotalPrice = request.TotalPrice,
                NumberOfGuests = request.Capacity,
                BookingDate = request.BookingDate,
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return _mapper.Map<BookingResponse>(booking);
        }
        public async Task<bool> RefundToOwnerAsync()
        {
            var currentDate = DateTime.UtcNow;
            var completedBookings = await _context.Bookings
                .Where(b => b.ApprovalStatus == ApprovalStatusBooking.Complete && b.CheckOutDate < currentDate)
                .ToListAsync();
            var roleId = new Guid("8d9f4a75-1f4e-471d-09f5-08dd0220a5d1");
            var adminRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.RoleId == roleId);
            if (adminRole == null)
            {
                _logger.LogError("Admin role not found.");
                return false;
            }

            var admin = await _context.Users.FindAsync(adminRole.UserId);
            if (admin == null)
            {
                _logger.LogError("Admin user not found.");
                return false;
            }

            foreach (var booking in completedBookings)
            {
                var ownerId = _villaRepository.GetOwnerIdByVillaId(booking.VillaId);
                if (ownerId == Guid.Empty)
                {
                    _logger.LogError("Không tìm thấy ownerId: " + booking.VillaId);
                    continue;
                }
                var owner = await _userRepository.GetByIdAsync(ownerId);
                if (owner == null)
                {
                    _logger.LogError("s: " + ownerId);
                    continue; 
                }

                var refundAmount = booking.TotalPrice * 0.92m;
                if (admin.Balance < refundAmount)
                {
                    _logger.LogError($" {refundAmount}, Available: {admin.Balance}");
                    continue;
                }
                admin.Balance -= refundAmount;
                owner.Balance += refundAmount;

                booking.ApprovalStatus = ApprovalStatusBooking.Canceled;

                _logger.LogInformation($"Refunded {refundAmount} to owner {ownerId} for booking {booking.Id}. Admin balance updated.");
            }

            // Lưu thay đổi vào database
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> CancelBookingAsync(Guid bookingId)
        {
            try
            {
                // Tìm booking trong cơ sở dữ liệu
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null)
                {
                    return false;
                }

                // Kiểm tra nếu booking đã thành công (ApprovalStatus = Success)
                if (booking.ApprovalStatus == ApprovalStatusBooking.Complete)
                {
                    // Lấy số tiền của TotalPrice
                    decimal totalPrice = booking.TotalPrice;

                    // Kiểm tra nếu hủy trong vòng 2 phút
                    var timeDifference = DateTime.UtcNow - booking.BookingDate;
                    bool isWithinTwoMinutes = timeDifference <= TimeSpan.FromMinutes(2);

                    decimal adminReceivedAmount = 0;
                    decimal villaOwnerAmount = 0;
                    decimal customerRefundAmount = 0;
                    var roleId = new Guid("8d9f4a75-1f4e-471d-09f5-08dd0220a5d1");
                    var adminRole = await _context.UserRoles.FirstOrDefaultAsync(u => u.RoleId == roleId);
                    if (adminRole == null)
                    {
                        _logger.LogError("Admin role not found.");
                        return false;
                    }
                    var admin = await _context.Users.FindAsync(adminRole.UserId);
                    if (admin == null)
                    {
                        _logger.LogError("Admin not found.");
                        return false;
                    }
                    if (admin.Balance < totalPrice)
                    {
                        _logger.LogError("Admin does not have enough balance.");
                        return false;
                    }
                    admin.Balance -= totalPrice;
                    if (isWithinTwoMinutes)
                    {
                        customerRefundAmount = totalPrice;
                    }
                    else
                    {
                        decimal refundAmount = totalPrice * 0.80m;
                        adminReceivedAmount = refundAmount * 0.10m;
                        villaOwnerAmount = refundAmount * 0.10m;
                        customerRefundAmount = refundAmount;
                    }
                  
                    var ownerId = _villaRepository.GetOwnerIdByVillaId(booking.VillaId);
                    if (ownerId == Guid.Empty)
                    {
                        _logger.LogError("OwnerId not found for VillaId: " + booking.VillaId);
                        return false;
                    }

                    var owner = await _userRepository.GetByIdAsync(ownerId);
                    if (owner == null)
                    {
                        _logger.LogError("Owner not found for OwnerId: " + ownerId);
                        return false;
                    }

                   

                    
                    // Cập nhật số dư cho chủ villa và khách hàng
                    owner.Balance += villaOwnerAmount;

                    // Trừ số tiền của admin từ Balance của admin
                    

                    // Cập nhật số dư của khách hàng
                    var user = await _context.Users.FindAsync(booking.UserId);
                    if (user == null)
                    {
                        _logger.LogError("User not found.");
                        return false;
                    }
                    user.Balance += customerRefundAmount;

                    // Cập nhật trạng thái booking thành hủy
                    booking.ApprovalStatus = ApprovalStatusBooking.Canceled;
                    _context.Bookings.Update(booking);

                    // Lưu thay đổi vào cơ sở dữ liệu
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Nếu booking chưa thành công, trả về false (hoặc xử lý riêng nếu cần)
                    return false;
                }

                // Trả về kết quả sau khi hoàn tiền hoặc hủy
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while cancelling booking and processing refund.");
                return false;
            }
        }



        public async Task<BookingResponse> GetBookingByIdAsync(Guid bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            return _mapper.Map<BookingResponse>(booking);
        }

        public async Task<IEnumerable<BookingResponse>> GetBookingByUserIdAsync(Guid userId)
        {
            var bookings = await _bookingRepository.GetAllBookingByUserId(userId);
            return _mapper.Map<IEnumerable<BookingResponse>>(bookings);
        }

        public async Task<IEnumerable<BookingResponse>> GetAllBookingAsync()
        {
            var bookings = await _context.Bookings.Include(v => v.Villa).Include(v => v.User)
               .Select(bookings => new BookingResponse
               {
                   BookingId = bookings.Id,
                   UserId = bookings.UserId,
                   CheckInDate = bookings.CheckInDate,
                   CheckOutDate = bookings.CheckOutDate,
                   TotalPrice = bookings.TotalPrice,
                   FullName = bookings.User.FullName,
                   VillaName = bookings.Villa.Name,
                   ApprovalStatus = bookings.ApprovalStatus,
               })
               .ToListAsync();

            return bookings;
        }
    }
}
