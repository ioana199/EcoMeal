using EcoMeal.Entities;

namespace EcoMeal.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<List<Order>> GetAll();
        public Task<Order?> GetById(Guid id);
        public Task AddToCartAsync(string userId, Guid packageId, int quantity);
    }
}
