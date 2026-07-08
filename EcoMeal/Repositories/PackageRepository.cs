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
            return await context.Packages.Include(p => p.PackageType).ToListAsync();
        }

        public async Task<Package?> GetByIdAsync(Guid id)
        {
            return await context.Packages.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Package package)
        {
            await context.Packages.AddAsync(package);
           // await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Package package)
        {
            context.Packages.Update(package);
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
