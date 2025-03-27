using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Model;

namespace DACN_VILLA.Interface.Repository
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Location>> GetAllAsync();
        Task<IEnumerable<LocationWithVillaCount>> GetLocationsWithApprovedVillaCountAsync();
        Task<Location> GetByIdAsync(Guid id);
        Task AddAsync(Location location);
        Task UpdateAsync(Location location);
        Task DeleteAsync(Guid id);
    }

}
