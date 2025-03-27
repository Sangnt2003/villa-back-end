﻿using DACN_VILLA.Interface.Repository;
using DACN_VILLA.Model;
using Microsoft.EntityFrameworkCore;
using System;

namespace DACN_VILLA.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Services>> GetAllAsync()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<Services> GetByIdAsync(Guid id)
        {
            return await _context.Services.FindAsync(id);
        }

        public async Task AddAsync(Services service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Services service)
        {
            _context.Services.Update(service);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }
        }
    }

}
