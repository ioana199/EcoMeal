using EcoMeal.Entities;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Repositories.Interfaces
{
    public interface IBusinessRepository
    {
        public Task<List<Business>> GetAllAsync();
        public Task<Business?> GetByIdAsync(Guid? id);
        public Task AddAsync(Business business);
        public Task DeleteAsync(Guid id);
        public Task<Business?> GetByManagerAsync(string managerId);

        public Task SaveChangesAsync();

    }
}
