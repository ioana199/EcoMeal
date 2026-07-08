using EcoMeal.Entities;
using EcoMeal.Repositories.Interfaces;
using EcoMeal.Services.Interfaces;

namespace EcoMeal.Services
{
    public class PackageService(IPackageRepository packageRepository) : IPackageService
    {
        public async Task<List<Package>> GetAll()
        {
            return await packageRepository.GetAllAsync();
        }
        public async Task<Package?> GetById(Guid id)
        {
            return await packageRepository.GetByIdAsync(id);    
        }
        public async Task Add(Package package)
        {
            await packageRepository.AddAsync(package);
            await packageRepository.SaveChangesAsync();
        }
    }
}
