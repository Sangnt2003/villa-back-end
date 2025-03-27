namespace DACN_VILLA.Interface.Repository
{
    public interface IServiceRepository
    {
        Task<IEnumerable<Services>> GetAllAsync();
        Task<Services> GetByIdAsync(Guid id);
        Task AddAsync(Services service);
        Task UpdateAsync(Services service);
        Task DeleteAsync(Guid id);
    }

}
