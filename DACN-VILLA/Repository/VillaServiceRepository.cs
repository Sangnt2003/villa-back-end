using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace DACN_VILLA.Repository
{
    public class VillaServiceRepository : IVillaServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public VillaServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VillaServices>> GetAllAsync()
        {
            return await _context.VillaServices.ToListAsync();
        }

        public async Task<VillaServices> GetByIdAsync(Guid villaId, Guid serviceId)
        {
            return await _context.VillaServices.FirstOrDefaultAsync(vs => vs.VillaId == villaId && vs.ServiceId == serviceId);
        }

        public async Task AddAsync(VillaServices villaService)
        {
            _context.VillaServices.Add(villaService);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VillaServices villaService)
        {
            _context.VillaServices.Update(villaService);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid villaId, Guid serviceId)
        {
            var villaService = await _context.VillaServices
                .FirstOrDefaultAsync(vs => vs.VillaId == villaId && vs.ServiceId == serviceId);
            if (villaService != null)
            {
                _context.VillaServices.Remove(villaService);
                await _context.SaveChangesAsync();
            }
        }
    }

}
