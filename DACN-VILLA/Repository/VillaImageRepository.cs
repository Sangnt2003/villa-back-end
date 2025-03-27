using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;

namespace DACN_VILLA.Repository
{
    public class VillaImageRepository : IVillaImageRepository
    {
        private readonly ApplicationDbContext _context;

        public VillaImageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VillaImage villaImage)
        {
            await _context.VillaImages.AddAsync(villaImage);
            await _context.SaveChangesAsync();
        }

        public async Task<VillaImage> GetByIdAsync(Guid id)
        {
            return await _context.VillaImages.FindAsync(id);
        }

        public async Task<List<VillaImage>> GetByVillaIdAsync(Guid villaId)
        {
            return await _context.VillaImages
                .Where(v => v.VillaId == villaId)
                .ToListAsync();
        }
    }
}
