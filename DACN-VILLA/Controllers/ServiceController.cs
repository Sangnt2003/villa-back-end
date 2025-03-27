using DACN_VILLA.DTO.Respone;
using DACN_VILLA.Interface.Service;
using DACN_VILLA.Model;
using DACN_VILLA.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DACN_VILLA.Controllers
{
    [ApiController]
    [Route("api/service")]
    public class ServiceController : ControllerBase
    {
        private readonly IService _serviceProvider;
        public ServiceController(IService service) 
        {
            _serviceProvider = service;
        }

        [HttpGet("all")]
        public async Task<ActionResult<ServiceListResponse>> GetAllServices(int pageNumber = 1, int pageSize = 16) 
        {
            var serviceResponses = await _serviceProvider.GetAllServicesAsync();
            int totalServices = serviceResponses.Count();


            var pagedServices = serviceResponses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new ServiceListResponse
            {
                Services = pagedServices,
                TotalPage = (int)Math.Ceiling((double)totalServices / pageSize) // Calculate total pages
            };

            return Ok(response);
        }
        [HttpGet("services")]
        public async Task<ActionResult<IEnumerable<ServiceResponse>>> GetAll()
        {
            var services = await _serviceProvider.GetAllServicesAsync();
            return Ok(services);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse>> GetServiceById(Guid id)
        {
            var service = await _serviceProvider.GetServiceByIdAsync(id);
            return Ok(service);
        }

        [HttpPost]
        public async Task<ActionResult> AddService([FromBody] ServiceResponse serviceDTO)
        {
            await _serviceProvider.AddServiceAsync(serviceDTO);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateService(Guid id, [FromBody] ServiceResponse serviceDTO)
        {
            if (id != serviceDTO.Id)
                return BadRequest("Villa ID mismatch");

            await _serviceProvider.UpdateServiceAsync(serviceDTO);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteService(Guid id)
        {
            await _serviceProvider.DeleteServiceAsync(id);
            return Ok();
        }
    }
}
