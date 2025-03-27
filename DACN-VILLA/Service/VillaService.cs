using AutoMapper;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.DTO.Response;
using DACN_VILLA.Helper;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using DACN_VILLA.Model.Enum;
using DACN_VILLA.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Quic;
using System.Threading.Tasks;

namespace DACN_VILLA.Service
{
    public class VillaService : IVillaService
    {
        private readonly IVillaRepository _villaRepository;
        private readonly IVillaImageRepository _villaImageRepository;
        private readonly ILocationService _locationService;
        private readonly IMapper _mapper;
        private readonly IServiceRepository _serviceRepository;
        private readonly IVillaServiceRepository _villaServiceRepository;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VillaService> _logger;
        private readonly IUserRepository _userRepository;
        public VillaService(IVillaRepository villaRepository, 
            ILogger<VillaService> logger, IMapper mapper, 
            ApplicationDbContext context, 
            ILocationService locationService, 
            IVillaImageRepository villaImageRepository,
            IVillaServiceRepository villaServiceRepository, 
            IServiceRepository serviceRepository,
            IUserRepository userRepository)
        {
            _villaRepository = villaRepository;
            _locationService = locationService;
            _logger = logger;
            _mapper = mapper;
            _context = context;
            _villaImageRepository = villaImageRepository;
            _villaServiceRepository = villaServiceRepository;
            _serviceRepository = serviceRepository;
            _userRepository = userRepository;
        }
        public async Task<IEnumerable<VillaResponse>> GetVillasNearbyUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) throw new Exception("User not found");

            var userAddress = AddressHelper.NormalizeAddress(user.Address);

            var villas = await _villaRepository.GetAllAsync();

            var matchingVillas = villas.Where(villa =>
            {
                var villaAddress = AddressHelper.NormalizeAddress(villa.Address);
                return villaAddress.Contains(userAddress); // So sánh trực tiếp
            }).ToList();


            return _mapper.Map<IEnumerable<VillaResponse>>(matchingVillas);
        }
        public async Task<IEnumerable<VillaResponse>> GetAllVillasAsync()
        {
            var villas = await _villaRepository.GetAllAsync(); 
            foreach (var villa in villas)
            {
                villa.VillaImages ??= new List<VillaImage>();
                villa.VillaServices ??= new List<VillaServices>();
                villa.VillaServices = villa.VillaServices.Where(vs => vs.Service != null).ToList(); // Exclude invalid VillaServices
            }
            var villaResponses = _mapper.Map<IEnumerable<VillaResponse>>(villas);

            var locationIds = villas.Select(v => v.LocationId).Distinct().ToList();

            var locationDictionary = new Dictionary<Guid, string>();
            var discountDictionary = new Dictionary<Guid, string>();

            foreach (var locationId in locationIds)
            {
                var location = await _locationService.GetLocationByIdAsync(locationId);
                if (location != null)
                {
                    locationDictionary[locationId] = location.Name;
                }
            }
            // Populate additional fields for each villa response
            foreach (var villaResponse in villaResponses)
            {
                var villa = villas.First(v => v.Id == villaResponse.Id);
                villaResponse.ImageUrls = villa.VillaImages.Select(i => i?.ImageUrl ?? string.Empty).Where(url => !string.IsNullOrEmpty(url)).ToList();
                villaResponse.LocationName = locationDictionary.TryGetValue(villaResponse.LocationId, out var locationName)
                    ? locationName
                    : "Unknown Location";
 
                villaResponse.VillaServices = villa.VillaServices.Select(vs => vs.Service?.Name ?? "Unknown Service").ToList();
                villaResponse.ApprovalStatus = villa.ApprovalStatus.ToString();
            }

            return villaResponses;
        }
        public async Task<IEnumerable<VillaResponse>> GetApprovedVillasAsync()
        {
            var villas = await _villaRepository.GetApprovedVillasAsync();
            var villaResponses = _mapper.Map<IEnumerable<VillaResponse>>(villas);
            return villaResponses;
        }
        public async Task<VillaResponse> GetVillaByIdAsync(Guid id)
        {
            var villa = await _villaRepository.GetByIdAsync(id);
            if (villa == null)
                throw new KeyNotFoundException("Villa not found.");

            villa.VillaImages ??= new List<VillaImage>();
            villa.VillaServices ??= new List<VillaServices>();
            villa.VillaServices = villa.VillaServices.Where(vs => vs.Service != null).ToList();

            var villaResponse = _mapper.Map<VillaResponse>(villa);
            villaResponse.ImageUrls = villa.VillaImages
                .Select(i => i?.ImageUrl ?? string.Empty)
                .Where(url => !string.IsNullOrEmpty(url))
                .ToList();

            villaResponse.ApprovalStatus = villa.ApprovalStatus.ToString();
            return villaResponse;
        }

        public async Task<bool> ApproveVillaAsync(Guid villaId, ApprovalStatus status)
        {
            var villa = await _villaRepository.GetByIdAsync(villaId);
            if (villa == null) return false;

            villa.ApprovalStatus = status;

            await _villaRepository.UpdateAsync(villa);
            return true;
        }

        public async Task<VillaResponse> AddVillaAsync(VillaCreateRequest villaRequest)
        {
            var villa = _mapper.Map<Villa>(villaRequest);
            villa.PricePerNight = villa.ListedPrice * 0.92m;
            villa.ApprovalStatus = ApprovalStatus.Pending;

            await _villaRepository.AddAsync(villa);

            if (villaRequest.ImageUrls != null && villaRequest.ImageUrls.Any())
            {
                foreach (var imageUrl in villaRequest.ImageUrls)
                {
                    var villaImage = new VillaImage
                    {
                        VillaId = villa.Id,
                        ImageUrl = imageUrl
                    };

                    await _villaImageRepository.AddAsync(villaImage);
                }
            }

            if (villaRequest.VillaServiceIds != null && villaRequest.VillaServiceIds.Any())
            {
                foreach (var serviceId in villaRequest.VillaServiceIds)
                {
                    var villaService = new VillaServices
                    {
                        VillaId = villa.Id,
                        ServiceId = serviceId,
                        IsAvailable = true
                    };
                    await _villaServiceRepository.AddAsync(villaService);
                }
            }

            var response = _mapper.Map<VillaResponse>(villa);
            response.VillaServices = villaRequest.VillaServiceIds
                .Select(id => _serviceRepository.GetByIdAsync(id).Result.Name)
                .ToList();
            return response;
        }

        public async Task DeleteVillaAsync(Guid id)
        {
            await _villaRepository.DeleteAsync(id);
        }

        public async Task<VillaResponse> UpdateVillaAsync(VillaUpdateRequest villaRequest)
        {
            var existingVilla = await _villaRepository.GetByIdAsync(villaRequest.Id);
            if (existingVilla == null)
                throw new KeyNotFoundException($"Villa with ID {villaRequest.Id} not found.");

            if (existingVilla.ApprovalStatus == ApprovalStatus.Pending)
                throw new InvalidOperationException("Cannot update a pending villa. Please approve or reject it first.");

            _mapper.Map(villaRequest, existingVilla);

            await _villaRepository.UpdateAsync(existingVilla);
            return _mapper.Map<VillaResponse>(existingVilla);
        }

        public async Task<List<VillaResponse>> SearchAvailableVillas(VillaSearchRequest request)
        {
            var query = _context.Villas.AsQueryable();

            if (request.LocationId != Guid.Empty)
            {
                query = query.Where(v => v.LocationId == request.LocationId);
                _logger.LogInformation($"Đã lọc theo LocationId: {request.LocationId}");
            }

            if (request.Capacity > 0)
            {
                query = query.Where(v => v.Capacity >= request.Capacity);
                _logger.LogInformation($"Đã lọc theo Capacity: {request.Capacity}");
            }

            var bookedVillaIds = await _context.Bookings
                .Where(b => b.BookingProcesses.Any(bp => bp.ApprovalStatus == ApprovalStatusBooking.Complete) &&
                            b.CheckInDate < request.EndDate &&
                            b.CheckOutDate > request.StartDate)
                .Select(b => b.VillaId)
                .ToListAsync();
            _logger.LogInformation($"Villa đã được đặt: {string.Join(", ", bookedVillaIds)}");

            var availableVillas = await query
                .Where(v => v.ApprovalStatus == ApprovalStatus.Approved && !bookedVillaIds.Contains(v.Id))
                .Include(v => v.Location)
                .Include(v => v.VillaImages)
                .Include(v => v.VillaServices)
                    .ThenInclude(vs => vs.Service)
                .Include(v => v.Bookings)
                .ToListAsync();
            _logger.LogInformation($"Số lượng Villa khả dụng: {availableVillas.Count}");
            return _mapper.Map<List<VillaResponse>>(availableVillas);
        }

        public async Task<IEnumerable<VillaResponse>> FilterVillas(VillaFilterRequest request)
        {
            var query = _context.Villas.AsQueryable();

            if (request.MinPrice.HasValue)
            {
                query = query.Where(v => v.ListedPrice >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(v => v.ListedPrice <= request.MaxPrice.Value);
            }

            if (request.Capacity.HasValue)
            {
                query = query.Where(v => v.Capacity >= request.Capacity.Value);
            }

            if (request.LocationId.HasValue)
            {
                query = query.Where(v => v.LocationId == request.LocationId.Value);
            }

            if (request.Rating.HasValue)
            {
                query = query.Where(v => v.Rating >= request.Rating.Value);
            }

            if (request.StartDate.HasValue && request.EndDate.HasValue)
            {
                var bookedVillas = _context.Bookings
                    .Where(b => b.CheckInDate < request.EndDate.Value && b.CheckOutDate > request.StartDate.Value)
                    .Select(b => b.VillaId)
                    .Distinct();

                query = query.Where(v => !bookedVillas.Contains(v.Id));
            }

            query = query.Where(v => v.ApprovalStatus == request.ApprovalStatus);
            var villas = await query
                .Include(v => v.Location)
                .Include(v => v.VillaImages)
                .Include(v => v.VillaServices)
                    .ThenInclude(vs => vs.Service)
                .ToListAsync();

            var villaResponses = _mapper.Map<IEnumerable<VillaResponse>>(villas);

            return villaResponses;
        }

        public async Task<IEnumerable<VillaResponse>> GetAllVillaByUserIdAsync(Guid userId)
        {
            var villas = await _villaRepository.GetAllVillaByUserId(userId);
            foreach (var villa in villas)
            {
                villa.VillaImages ??= new List<VillaImage>();
                villa.VillaServices ??= new List<VillaServices>();
                villa.VillaServices = villa.VillaServices.Where(vs => vs.Service != null).ToList();
            }
            var villaResponses = _mapper.Map<IEnumerable<VillaResponse>>(villas);
            var locationIds = villas.Select(v => v.LocationId).Distinct().ToList();
            var locationDictionary = new Dictionary<Guid, string>();

            foreach (var locationId in locationIds)
            {
                var location = await _locationService.GetLocationByIdAsync(locationId);
                if (location != null)
                {
                    locationDictionary[locationId] = location.Name;
                }
            }

            foreach (var villaResponse in villaResponses)
            {
                var villa = villas.First(v => v.Id == villaResponse.Id);
                villaResponse.ImageUrls = villa.VillaImages.Select(i => i?.ImageUrl ?? string.Empty).Where(url => !string.IsNullOrEmpty(url)).ToList();
                villaResponse.LocationName = locationDictionary.TryGetValue(villaResponse.LocationId, out var locationName)
                    ? locationName
                    : "Unknown Location";
                villaResponse.VillaServices = villa.VillaServices.Select(vs => vs.Service?.Name ?? "Unknown Service").ToList();
                villaResponse.ApprovalStatus = villa.ApprovalStatus.ToString();
            }
            return villaResponses;
        }
    }
}
