using EcoMeal.Database;
using EcoMeal.Entities;
using EcoMeal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Repositories
{
    public class BusinessRepository(EcoMealDBContext context) : IBusinessRepository
    {
        public async Task<List<Business>> GetAllAsync()
        {
            return await context.Businesses.Include(b=> b.BusinessType).ToListAsync();
        }

        public async Task<Business?> GetByIdAsync(Guid? id)
        {
            return await context.Businesses.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task AddAsync(Business business)
        {
            await context.Businesses.AddAsync(business);
        }

        public async Task DeleteAsync(Guid id)
        {
            var business = await GetByIdAsync(id);
            if (business is null)
                return;
            context.Businesses.Remove(business);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

    }
}
