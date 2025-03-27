using AutoMapper;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;

namespace DACN_VILLA.Service
{
    public class ServiceProvider : IService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper _mapper;

        public ServiceProvider(IServiceRepository serviceRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ServiceResponse>> GetAllServicesAsync()
        {
            var services = await _serviceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ServiceResponse>>(services);
        }

        public async Task<ServiceResponse> GetServiceByIdAsync(Guid id)
        {
            var service = await _serviceRepository.GetByIdAsync(id);
            if (service == null)
                throw new KeyNotFoundException($"Service with ID {id} not found");

            return _mapper.Map<ServiceResponse>(service);
        }

        public async Task AddServiceAsync(ServiceResponse serviceDto)
        {
            var service = _mapper.Map<Services>(serviceDto);
            await _serviceRepository.AddAsync(service);
        }

        public async Task UpdateServiceAsync(ServiceResponse serviceDto)
        {
            var service = _mapper.Map<Services>(serviceDto);
            await _serviceRepository.UpdateAsync(service);
        }

        public async Task DeleteServiceAsync(Guid id)
        {
            await _serviceRepository.DeleteAsync(id);
        }
    }
}
