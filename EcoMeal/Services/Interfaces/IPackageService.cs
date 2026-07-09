using EcoMeal.Entities;

namespace EcoMeal.Services.Interfaces
{
    public interface IPackageService
    {
        public Task<List<Package>> GetAll();
        public Task<Package?> GetById(Guid? id);
        public Task Add(Package package);
        public Task<Package?> Update(Package package, Guid id);
        public Task Delete(Guid id);
    }
}
