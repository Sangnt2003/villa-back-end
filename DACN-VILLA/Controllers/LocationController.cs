using DACN_VILLA.DTO;
using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.DTO.Response;
using DACN_VILLA.Interface.Service;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DACN_VILLA.Controllers
{
    [ApiController]
    [Route("api/location")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IVillaService _villaService;

        public LocationController(ILocationService locationService, 
            IVillaService villaService)
        {
            _locationService = locationService;
            _villaService = villaService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<LocationListResponse>> GetAllLocations(int pageNumber = 1, int pageSize = 21)
        {
            var locations = await _locationService.GetAllLocationsAsync();
            var pagedLocations = locations
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new LocationListResponse
            {
                Locations = pagedLocations,
                TotalPages = (int)Math.Ceiling((double)locations.Count() / pageSize)
            };

            return Ok(response);
        }

        [HttpGet("locations")]
        public async Task<ActionResult<IEnumerable<LocationResponse>>> GetAll()
        {
            var locations = await _locationService.GetAllLocationsAsync();
            return Ok(locations);
        }

        [HttpGet("location-villa")]
        public async Task<ActionResult<IEnumerable<LocationWithVillaCount>>> GetAllLocations()
        {
            var locations = await _locationService.GetAllLocationsWithVillaCountAsync();
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LocationResponse>> GetLocationById(Guid id)  
        {
            var location = await _locationService.GetLocationByIdAsync(id);
            if (location == null)
                return NotFound($"Location with ID {id} not found.");

            return Ok(location);
        }

        [HttpPost]
        public async Task<ActionResult> AddLocation([FromBody] LocationCreateRequest locationDto)
        {
            var createdLocation = await _locationService.AddLocationAsync(locationDto);
            return CreatedAtAction(nameof(GetLocationById), new { id = createdLocation.Id }, createdLocation);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateLocation(Guid id, [FromBody] LocationUpdateRequest locationDto)
        {
            var existingLocation = await _locationService.GetLocationByIdAsync(id);
            if (existingLocation == null)
                return NotFound($"Location with ID {id} not found.");

            locationDto.Id = id; // Assign ID from URL to DTO
            var updatedLocation = await _locationService.UpdateLocationAsync(locationDto);
            return Ok(updatedLocation);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteLocation(Guid id)
        {
            var existingLocation = await _locationService.GetLocationByIdAsync(id);
            if (existingLocation == null)
                return NotFound($"Location with ID {id} not found.");

            await _locationService.DeleteLocationAsync(id);
            return Ok("Location successfully deleted.");
        }
    }
}
