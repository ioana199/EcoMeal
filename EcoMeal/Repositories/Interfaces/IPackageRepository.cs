using EcoMeal.Entities;

namespace EcoMeal.Repositories.Interfaces
{
    public interface IPackageRepository
    {
        public Task<List<Package>> GetAllAsync();
        public Task<Package?> GetByIdAsync(Guid? id);
        public Task AddAsync(Package package);
        public Task DeleteAsync(Guid id);
        public Task SaveChangesAsync();

    }
}
