using EcoMeal.Entities;

namespace EcoMeal.Services.Interfaces
{
    public interface IPackageService
    {
        public Task<List<Package>> GetAll();
        public Task<List<Package>> GetForManagement(string callerUserId, bool isAdmin);
        public Task<Package?> GetById(Guid? id);
        public Task Add(Package package, string callerUserId, bool isAdmin);
        public Task<Package?> Update(Package package, Guid id, string callerUserId, bool isAdmin);
        public Task Delete(Guid id, string callerUserId, bool isAdmin);
    }
}