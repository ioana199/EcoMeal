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

        public async Task<Order?> GetCartWithItemsAsync(string userId)
        {
            return await context.Orders
                .Include(o => o.OrderPackages)
                    .ThenInclude(op => op.Package)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.StatusId == StatusEnum.New);
        }

        public Task DeleteAsync(Order order)
        {
            context.Orders.Remove(order);
            return Task.CompletedTask;
        }

        public async Task<int> GetNextOrderNumberAsync()
        {
            var numbers = await context.Orders
                .Select(o => o.OrderNumber)
                .ToListAsync();

            var max = numbers
                .Select(n => int.TryParse(n, out var v) ? v : 0)
                .DefaultIfEmpty(0)
                .Max();

            return max + 1;
        }

        public async Task<List<Order>> GetByUserWithItemsAsync(string userId)
        {
            return await context.Orders
                .Include(o => o.Status)
                .Include(o => o.Business)
                .Include(o => o.OrderPackages)
                    .ThenInclude(op => op.Package)
                .Where(o => o.UserId == userId && o.StatusId != StatusEnum.New)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdWithItemsAsync(Guid id)
        {
            return await context.Orders
                .Include(o => o.OrderPackages)
                    .ThenInclude(op => op.Package)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> GetAllWithItemsAsync()
        {
            return await context.Orders
                .Include(o => o.Status)
                .Include(o => o.Business)
                .Include(o => o.User)
                .Include(o => o.OrderPackages)
                    .ThenInclude(op => op.Package)
                .ToListAsync();
        }

        public async Task<List<Order>> GetByBusinessWithItemsAsync(Guid businessId)
        {
            return await context.Orders
                .Include(o => o.Status)
                .Include(o => o.Business)
                .Include(o => o.User)
                .Include(o => o.OrderPackages)
                    .ThenInclude(op => op.Package)
                .Where(o => o.BusinessId == businessId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }

}
