using EcoMeal.Entities;

namespace EcoMeal.Repositories.Interfaces
{
    public interface IOrderPackageRepository
    {
        public Task<OrderPackage?> GetByOrderAndPackageAsync(Guid orderId, Guid packageId);
        public Task AddAsync(OrderPackage orderPackage);
        public Task SaveChangesAsync();

    }
}
