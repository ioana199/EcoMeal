using EcoMeal.Entities;
using EcoMeal.Entities.Enums;
using EcoMeal.Repositories;
using EcoMeal.Repositories.Interfaces;
using EcoMeal.Services.Interfaces;

namespace EcoMeal.Services
{
    public class OrderService(IOrderRepository orderRepository, IOrderPackageRepository orderPackageRepository, IPackageRepository packageRepository) : IOrderService 
    {
        public async Task<List<Order>> GetAll()
        {
            return await orderRepository.GetAllAsync();
        }

        public async Task<Order?> GetById(Guid id)
        {
            return await orderRepository.GetByIdAsync(id);
        }

        public async Task AddToCartAsync(string userId, Guid packageId, int quantity)
        {
            var package = await packageRepository.GetByIdAsync(packageId)
                ?? throw new InvalidOperationException("Pachetul nu mai există.");

            var cart = await orderRepository.GetOrderWithStatusNew(userId);

            if (cart is null)
            {
                cart = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    BusinessId = package.BusinessId,
                    StatusId = StatusEnum.New,
                    OrderNumber = "0"
                };
                await orderRepository.AddAsync(cart);
            }
            else
            {
                if (cart.BusinessId != package.BusinessId)
                    throw new InvalidOperationException(
                        "Poți adăuga pachete doar de la un singur business. Finalizează comanda curentă întâi.");
            }

            var existingOrderWithPackageId = await orderPackageRepository.GetByOrderAndPackageAsync(cart.Id, packageId);

            if (existingOrderWithPackageId is not null)
            {
                existingOrderWithPackageId.Quantity += quantity;
            }
            else
            {
                await orderPackageRepository.AddAsync(new OrderPackage
                {
                    Id = Guid.NewGuid(),
                    OrderId = cart.Id,
                    PackageId = packageId,
                    Quantity = quantity
                });
            }

            await orderRepository.SaveChangesAsync();
        }
    }
}
