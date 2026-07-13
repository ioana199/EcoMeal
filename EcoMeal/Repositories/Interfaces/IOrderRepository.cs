using EcoMeal.Entities;

namespace EcoMeal.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        public Task<List<Order>> GetAllAsync();
        public Task<Order?> GetByIdAsync(Guid id);
        public Task AddAsync(Order order);
        public Task UpdateAsync(Order order);
        public Task DeleteAsync(Guid id);
        public Task SaveChangesAsync();
    }
}
