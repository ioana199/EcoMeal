using EcoMeal.Database;
using EcoMeal.Entities;
using EcoMeal.Entities.Enums;
using EcoMeal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Repositories
{
    public class OrderRepository(EcoMealDBContext context) : IOrderRepository
    { 
        public async Task<List<Order>> GetAllAsync()
        {
            return await context.Orders
                .Include(o => o.Status)
                .Include(o => o.Business)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await context.Orders
                .Include(o => o.Status)
                .Include(o => o.Business)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetOrderWithStatusNew(string userId)
        {
            return await context.Orders
                .FirstOrDefaultAsync(o => o.UserId == userId && o.StatusId == StatusEnum.New);
        }

        public async Task AddAsync(Order order)
        {
            await context.Orders.AddAsync(order);
        }

        public async Task DeleteAsync(Guid id)
        {
            var order = await GetByIdAsync(id);
            if (order is null)
                return;
            context.Orders.Remove(order);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }

}
