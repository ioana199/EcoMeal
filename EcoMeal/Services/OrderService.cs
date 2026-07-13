using EcoMeal.Entities;
using EcoMeal.Repositories;
using EcoMeal.Repositories.Interfaces;
using EcoMeal.Services.Interfaces;

namespace EcoMeal.Services
{
    public class OrderService(IOrderRepository orderRepository) : IOrderService 
    {
        public async Task<List<Order>> GetAll()
        {
            return await orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetById(Guid id)
        {
            return await orderRepository.GetByIdAsync(id);
        }

        public async Task Add(Order order)
        {
            await orderRepository.AddAsync(order);
            await orderRepository.SaveChangesAsync();
        }

        public async Task<Order> Update(Order order, Guid id)
        {
            Order existingOrder = await orderRepository.GetByIdAsync(id);

            existingOrder.OrderNumber = order.OrderNumber;
            existingOrder.UserId = order.UserId;
            existingOrder.User=order.User;
            existingOrder.BusinessId = order.BusinessId;
            existingOrder.Business=order.Business;
            existingOrder.Status=order.Status;

            await orderRepository.SaveChangesAsync();
            return existingOrder;
        }

        public async Task Delete(Guid id)
        {
            await orderRepository.DeleteAsync(id);
            await orderRepository.SaveChangesAsync();
        }
    }
}
