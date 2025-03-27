using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using DACN_VILLA.Model.Enum;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DACN_VILLA.Repository
{
    public class VillaRepository : IVillaRepository
    {
        private readonly ApplicationDbContext _context;

        public VillaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Villa>> GetAllAsync()
        {
            return await _context.Villas
                .Include(v => v.Location)
                .Include(v => v.VillaServices)
                .ThenInclude(vs => vs.Service)
                .Include(v => v.VillaImages)
                .ToListAsync();
        }
        public async Task<IEnumerable<Villa>> GetVillasByLocationAsync(string city, string district)
        {
            return await _context.Villas.Include(v => v.VillaImages).Include(v => v.VillaServices).ThenInclude(v => v.Service)
                .Where(v => v.Address.Contains(city) && v.Address.Contains(district) && v.ApprovalStatus == ApprovalStatus.Approved)
                .ToListAsync();
        }
        public Guid GetOwnerIdByVillaId(Guid villaId)
            {
                var villa = _context.Villas.FirstOrDefault(v => v.Id == villaId);
                return villa.UserId;
            }
            public async Task<IEnumerable<Villa>> GetApprovedVillasAsync()
            {
                return await _context.Villas
                    .Where(v => v.ApprovalStatus == ApprovalStatus.Approved)
                    .Include(v => v.Location)
                    .Include(v => v.VillaImages)
                    .Include(v => v.VillaServices)
                    .ThenInclude(v=> v.Service)
                    .ToListAsync();
            }
            public async Task<Villa> GetByIdAsync(Guid id)
            {
                return await _context.Villas
                    .Include(v => v.VillaServices)      
                        .ThenInclude(vs => vs.Service)
                        .Include(v => v.User)
                        .Include(v => v.Reviews)
                    .Include(v => v.VillaImages) 
                    .FirstOrDefaultAsync(v => v.Id == id);
            }

            public async Task AddAsync(Villa villa)
            {
                _context.Villas.Add(villa);
                await _context.SaveChangesAsync();
            }
            public async Task<IEnumerable<Villa>> GetAllVillaByUserId(Guid userId)
            {
                return await _context.Villas
                    .Where(v => v.UserId == userId)  
                    .Include(v => v.Location)           
                    .Include(v => v.VillaImages)     
                    .ToListAsync();                 
            }
            public async Task UpdateAsync(Villa villa)
            {
                _context.Villas.Update(villa);
                await _context.SaveChangesAsync();
            }

            public async Task DeleteAsync(Guid id)
            {
                var villa = await _context.Villas.FindAsync(id);
                if (villa == null)
                {
                    throw new KeyNotFoundException($"Villa with ID {id} not found."); // Throw exception if not found
                }

                _context.Villas.Remove(villa);
                await _context.SaveChangesAsync();
            }

        }
    }
