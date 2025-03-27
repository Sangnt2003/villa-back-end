using DACN_VILLA.Model;

namespace DACN_VILLA.Interface.Repository
{
    public interface IVillaServiceRepository
    {
        Task<IEnumerable<VillaServices>> GetAllAsync();
        Task<VillaServices> GetByIdAsync(Guid villaId, Guid serviceId);
        Task AddAsync(VillaServices villaService);
        Task UpdateAsync(VillaServices villaService);
        Task DeleteAsync(Guid villaId, Guid serviceId);
    }

}
