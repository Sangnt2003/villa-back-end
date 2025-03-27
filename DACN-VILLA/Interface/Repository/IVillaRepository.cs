using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DACN_VILLA.Interface.Repository
{
    public interface IVillaRepository
    {
        Task<IEnumerable<Villa>> GetAllAsync();
        Task<IEnumerable<Villa>> GetVillasByLocationAsync(string city, string district);
        Task<IEnumerable<Villa>> GetAllVillaByUserId(Guid userId);
        Task<Villa> GetByIdAsync(Guid id);
        Task AddAsync(Villa villa);
        Task UpdateAsync(Villa villa);
        Guid GetOwnerIdByVillaId(Guid villaId); 
        Task<IEnumerable<Villa>> GetApprovedVillasAsync();
        Task DeleteAsync(Guid id);
    }
}
