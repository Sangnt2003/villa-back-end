using DACN_VILLA.DTO.Respone;

namespace DACN_VILLA.Interface.Service
{
    public interface IService
    {
        Task<IEnumerable<ServiceResponse>> GetAllServicesAsync();
        Task<ServiceResponse> GetServiceByIdAsync(Guid id);
        Task AddServiceAsync(ServiceResponse serviceDTO);
        Task UpdateServiceAsync(ServiceResponse serviceDTO);
        Task DeleteServiceAsync(Guid id);
    }
}
