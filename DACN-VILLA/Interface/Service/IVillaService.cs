using DACN_VILLA.DTO.Request;
using DACN_VILLA.DTO.Respone;
using DACN_VILLA.DTO.Response;
using DACN_VILLA.Model;
using DACN_VILLA.Model.Enum;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DACN_VILLA.Interface.Service
{
    public interface IVillaService
    {
        Task<bool> ApproveVillaAsync(Guid villaId, ApprovalStatus status);
        Task<IEnumerable<VillaResponse>> GetAllVillasAsync();
        Task<IEnumerable<VillaResponse>> GetApprovedVillasAsync();
        Task<IEnumerable<VillaResponse>> GetAllVillaByUserIdAsync(Guid userId);
        Task<VillaResponse> GetVillaByIdAsync(Guid id);
        Task<VillaResponse> AddVillaAsync(VillaCreateRequest villaRequest);
        Task<IEnumerable<VillaResponse>> GetVillasNearbyUserAsync(Guid userId);
        Task<VillaResponse> UpdateVillaAsync(VillaUpdateRequest villaRequest);
        Task DeleteVillaAsync(Guid id);
        Task<List<VillaResponse>> SearchAvailableVillas(VillaSearchRequest request);
        Task<IEnumerable<VillaResponse>> FilterVillas(VillaFilterRequest request);
    }
}
