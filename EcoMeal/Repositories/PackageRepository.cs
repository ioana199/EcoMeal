using EcoMeal.Database;
using EcoMeal.Entities;
using EcoMeal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Repositories
{
    public class PackageRepository(EcoMealDBContext context) : IPackageRepository
    {
        public async Task<List<Package>> GetAllAsync()
        {
            return await context.Packages
                .Include(p => p.PackageType)
                .Include(p => p.Business)
                    .ThenInclude(b => b.BusinessType)
                .ToListAsync();
        }

        public async Task<Package?> GetByIdAsync(Guid? id)
        {
            if (id is null)
                return null;
            return await context.Packages.FirstOrDefaultAsync(p => p.Id == id.Value);
        }

        public async Task AddAsync(Package package)
        {
            await context.Packages.AddAsync(package);
        }

        public async Task DeleteAsync(Guid id)
        {
            var package = await GetByIdAsync(id);
            if (package is null)
                return;
            context.Packages.Remove(package);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
