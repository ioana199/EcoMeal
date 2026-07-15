using EcoMeal.Entities;

namespace EcoMeal.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<List<Order>> GetAll();
        public Task<Order?> GetById(Guid id);
        public Task AddToCartAsync(string userId, Guid packageId, int quantity);
        public Task<Order?> GetCartAsync(string userId);
        public Task UpdateQuantityAsync(string userId, Guid packageId, int quantity);
        public Task RemoveFromCartAsync(string userId, Guid packageId);
        public Task PlaceOrderAsync(string userId);
        public Task CancelCartAsync(string userId);
    }
}
