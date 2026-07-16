using EcoMeal.Entities;
using EcoMeal.Entities.Enums;

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
        public Task<List<Order>> GetMyOrdersAsync(string userId);
        public Task ChangeStatusAsync(Guid orderId, StatusEnum newStatus, string callerUserId, bool isAdmin);
        public Task DeleteOrderAsync(Guid orderId, string callerUserId, bool isAdmin);
        public Task<List<Order>> GetOrdersForManagementAsync(string callerUserId, bool isAdmin);
        public IEnumerable<StatusEnum> GetAllowedTransitions(StatusEnum current);

    }
}
