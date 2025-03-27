using AutoMapper;
using DACN_VILLA.DTO;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DACN_VILLA.Service
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository locationRepository, IMapper mapper, ApplicationDbContext context)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
            _context = context;
        }
        public async Task<IEnumerable<LocationWithVillaCount>> GetAllLocationsWithVillaCountAsync()
        {
            var locations = await _locationRepository.GetLocationsWithApprovedVillaCountAsync();
            return locations;
        }


        public async Task<int> CountByLocationAsync(Guid locationId)
        {
            return await _context.Villas.CountAsync(v => v.LocationId == locationId);
        }

        public async Task<IEnumerable<LocationResponse>> GetAllLocationsAsync()
        {
            var locations = await _context.Locations
                .Select(location => new LocationResponse
                {
                    Id = location.Id,
                    Name = location.Name,
                    TotalVilla = _context.Villas.Count(villa => villa.LocationId == location.Id),
                    ImageUrl = location.ImageUrl,
                    Description = location.Description,
                })
                .ToListAsync();

            return locations;
        }


        public async Task<LocationResponse> GetLocationByIdAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null)
                throw new KeyNotFoundException($"Location with ID {id} not found.");

            return _mapper.Map<LocationResponse>(location);
        }

        public async Task<LocationResponse> AddLocationAsync(LocationCreateRequest locationDto)
        {
            var location = _mapper.Map<Location>(locationDto);
            location.Id = Guid.NewGuid(); // Automatically generate a new ID
            await _locationRepository.AddAsync(location);
            return _mapper.Map<LocationResponse>(location);
        }

        public async Task<LocationResponse> UpdateLocationAsync(LocationUpdateRequest locationDto)
        {
            var existingLocation = await _locationRepository.GetByIdAsync(locationDto.Id);
            if (existingLocation == null)
                throw new KeyNotFoundException($"Location with ID {locationDto.Id} not found.");

            _mapper.Map(locationDto, existingLocation);
            await _locationRepository.UpdateAsync(existingLocation);

            return _mapper.Map<LocationResponse>(existingLocation);
        }

        public async Task DeleteLocationAsync(Guid id)
        {
            var existingLocation = await _locationRepository.GetByIdAsync(id);
            if (existingLocation == null)
                throw new KeyNotFoundException($"Location with ID {id} not found.");

            await _locationRepository.DeleteAsync(id);
        }
    }
}
