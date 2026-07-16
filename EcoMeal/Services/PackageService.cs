using EcoMeal.Entities;
using EcoMeal.Repositories.Interfaces;
using EcoMeal.Services.Interfaces;

namespace EcoMeal.Services
{
    public class PackageService(
        IPackageRepository packageRepository,
        IBusinessRepository businessRepository) : IPackageService
    {
        public async Task<List<Package>> GetAll()
        {
            return await packageRepository.GetAllAsync();
        }

        public async Task<List<Package>> GetForManagement(string callerUserId, bool isAdmin)
        {
            var all = await packageRepository.GetAllAsync();

            if (isAdmin)
                return all;

            var business = await businessRepository.GetByManagerAsync(callerUserId);
            if (business is null)
                return new List<Package>();

            return all.Where(p => p.BusinessId == business.Id).ToList();
        }

        public async Task<Package?> GetById(Guid? id)
        {
            if (id is null)
                return null;
            return await packageRepository.GetByIdAsync(id.Value);
        }

        public async Task Add(Package package, string callerUserId, bool isAdmin)
        {
            if (!isAdmin)
            {
                // managerul: forțează business-ul la al lui, indiferent ce a venit din formular
                var business = await businessRepository.GetByManagerAsync(callerUserId)
                    ?? throw new InvalidOperationException("Nu administrezi niciun business.");
                package.BusinessId = business.Id;
            }
            // admin: folosește business-ul ales în formular (package.BusinessId)

            await packageRepository.AddAsync(package);
            await packageRepository.SaveChangesAsync();
        }

        public async Task<Package?> Update(Package package, Guid id, string callerUserId, bool isAdmin)
        {
            var existingPackage = await packageRepository.GetByIdAsync(id);
            if (existingPackage is null)
                return null;

            if (!isAdmin)
            {
                var business = await businessRepository.GetByManagerAsync(callerUserId)
                    ?? throw new InvalidOperationException("Nu administrezi niciun business.");

                // poate edita doar pachetele business-ului lui
                if (existingPackage.BusinessId != business.Id)
                    throw new InvalidOperationException("Nu poți edita un pachet al altui business.");

                // și nu-l poate muta la alt business
                package.BusinessId = business.Id;
            }

            existingPackage.Name = package.Name;
            existingPackage.Description = package.Description;
            existingPackage.Price = package.Price;
            existingPackage.Quantity = package.Quantity;
            existingPackage.PickupStart = package.PickupStart;
            existingPackage.PickupEnd = package.PickupEnd;
            existingPackage.ImageUrl = package.ImageUrl;
            existingPackage.PackageTypeId = package.PackageTypeId;
            existingPackage.BusinessId = package.BusinessId;

            await packageRepository.SaveChangesAsync();
            return existingPackage;
        }

        public async Task Delete(Guid id, string callerUserId, bool isAdmin)
        {
            var existingPackage = await packageRepository.GetByIdAsync(id);
            if (existingPackage is null)
                return;

            if (!isAdmin)
            {
                var business = await businessRepository.GetByManagerAsync(callerUserId)
                    ?? throw new InvalidOperationException("Nu administrezi niciun business.");

                if (existingPackage.BusinessId != business.Id)
                    throw new InvalidOperationException("Nu poți șterge un pachet al altui business.");
            }

            await packageRepository.DeleteAsync(id);
            await packageRepository.SaveChangesAsync();
        }
    }
}