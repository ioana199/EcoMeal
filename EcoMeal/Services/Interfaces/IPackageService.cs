using EcoMeal.Entities;

namespace EcoMeal.Services.Interfaces
{
    public interface IPackageService
    {
        public Task<List<Package>> GetAll();
        public Task<Package?> GetById(Guid id);
        public Task Add(Package package);
    }
}
