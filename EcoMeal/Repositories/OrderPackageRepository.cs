using EcoMeal.Database;
using EcoMeal.Entities;
using EcoMeal.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Repositories
{
    public class OrderPackageRepository(EcoMealDBContext context) : IOrderPackageRepository
    {
        public async Task<OrderPackage?> GetByOrderAndPackageAsync(Guid orderId, Guid packageId)
        {
            return await context.OrderPackages
                .FirstOrDefaultAsync(op => op.OrderId == orderId && op.PackageId == packageId);
        }

        public async Task AddAsync(OrderPackage orderPackage)
        {
            await context.OrderPackages.AddAsync(orderPackage);
        }

        public Task DeleteAsync(OrderPackage orderPackage)
        {
            context.OrderPackages.Remove(orderPackage);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
