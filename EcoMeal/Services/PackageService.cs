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
        public async Task<Package?> GetById(Guid? id)
        {
            if (id is null)
                return null;
            return await packageRepository.GetByIdAsync(id.Value);
        }
        public async Task Add(Package package)
        {
            await packageRepository.AddAsync(package);
            await packageRepository.SaveChangesAsync();
        }

        public async Task<Package?> Update(Package package, Guid id)
        {
            var existingPackage = await packageRepository.GetByIdAsync(id);
            if (existingPackage is null)
                return null;

            existingPackage.Name = package.Name;
            existingPackage.Description = package.Description;
            existingPackage.Price = package.Price;
            existingPackage.Quantity = package.Quantity;
            existingPackage.PickupStart = package.PickupStart;
            existingPackage.PickupEnd = package.PickupEnd;
            existingPackage.ImageUrl = package.ImageUrl;
            existingPackage.PackageTypeId = package.PackageTypeId;
            existingPackage.BusinessId = package.BusinessId;
            existingPackage.PackageType = package.PackageType;

            await packageRepository.SaveChangesAsync();
            return existingPackage;
        }

        public async Task Delete(Guid id)
        {
            await packageRepository.DeleteAsync(id);
            await packageRepository.SaveChangesAsync();
        }
    }
}
