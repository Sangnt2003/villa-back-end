using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Model;

namespace DACN_VILLA.Interface.Service
{
    public interface ILocationService
    {
        Task<IEnumerable<LocationResponse>> GetAllLocationsAsync();
        Task<LocationResponse> GetLocationByIdAsync(Guid id);
        Task<LocationResponse> AddLocationAsync(LocationCreateRequest locationDto);
        Task<LocationResponse> UpdateLocationAsync(LocationUpdateRequest locationDto);
        Task<IEnumerable<LocationWithVillaCount>> GetAllLocationsWithVillaCountAsync();
        Task<int> CountByLocationAsync(Guid locationId);
        Task DeleteLocationAsync(Guid id);
    }

}
