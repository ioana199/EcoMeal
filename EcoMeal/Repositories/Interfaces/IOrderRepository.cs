using EcoMeal.Entities;

namespace EcoMeal.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        public Task<List<Order>> GetAllAsync();
        public Task<Order?> GetByIdAsync(Guid id);
        public Task<Order?> GetOrderWithStatusNew(string userId);
        public Task AddAsync(Order order);
        public Task DeleteAsync(Guid id);
        public Task SaveChangesAsync();
        public Task<Order?> GetCartWithItemsAsync(string userId);
        public Task DeleteAsync(Order order);
        public Task<int> GetNextOrderNumberAsync();
        public Task<List<Order>> GetByUserWithItemsAsync(string userId);
        public Task<Order?> GetByIdWithItemsAsync(Guid id);
        public Task<List<Order>> GetAllWithItemsAsync();
        public Task<List<Order>> GetByBusinessWithItemsAsync(Guid businessId);

    }
}
