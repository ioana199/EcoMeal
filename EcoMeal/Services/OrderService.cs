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

        public async Task<Order?> GetCartAsync(string userId)
        {
            return await orderRepository.GetCartWithItemsAsync(userId);
        }

        public async Task UpdateQuantityAsync(string userId, Guid packageId, int quantity)
        {
            if (quantity < 1)
                throw new InvalidOperationException("Cantitatea trebuie să fie cel puțin 1.");

            var cart = await orderRepository.GetCartWithItemsAsync(userId)
                ?? throw new InvalidOperationException("Nu ai niciun coș activ.");

            var line = cart.OrderPackages.FirstOrDefault(op => op.PackageId == packageId)
                ?? throw new InvalidOperationException("Pachetul nu e în coș.");

            line.Quantity = quantity;
            await orderRepository.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(string userId, Guid packageId)
        {
            var cart = await orderRepository.GetCartWithItemsAsync(userId)
                ?? throw new InvalidOperationException("Nu ai niciun coș activ.");

            var line = cart.OrderPackages.FirstOrDefault(op => op.PackageId == packageId);
            if (line is null)
                return;

            await orderPackageRepository.DeleteAsync(line);

            // Dacă a rămas gol coșul, șterge și comanda
            if (cart.OrderPackages.Count == 1)
                await orderRepository.DeleteAsync(cart);

            await orderRepository.SaveChangesAsync();
        }

        public async Task PlaceOrderAsync(string userId)
        {
            var cart = await orderRepository.GetCartWithItemsAsync(userId)
                ?? throw new InvalidOperationException("Nu ai niciun coș activ.");

            if (cart.OrderPackages.Count == 0)
                throw new InvalidOperationException("Coșul este gol.");

            // Reverifică stocul pentru fiecare linie
            foreach (var line in cart.OrderPackages)
            {
                if (line.Package.Quantity < line.Quantity)
                    throw new InvalidOperationException(
                        $"Stoc insuficient pentru {line.Package.Name} (disponibil: {line.Package.Quantity}).");
            }

            // Scade stocul
            foreach (var line in cart.OrderPackages)
            {
                line.Package.Quantity -= line.Quantity;
            }

            // Număr secvențial + status Reserved
            var nextNumber = await orderRepository.GetNextOrderNumberAsync();
            cart.OrderNumber = nextNumber.ToString();
            cart.StatusId = StatusEnum.Reserved;

            await orderRepository.SaveChangesAsync();
        }

        public async Task CancelCartAsync(string userId)
        {
            var cart = await orderRepository.GetCartWithItemsAsync(userId);
            if (cart is null)
                return;

            // Întâi liniile, apoi comanda (din cauza FK)
            foreach (var line in cart.OrderPackages.ToList())
                await orderPackageRepository.DeleteAsync(line);

            await orderRepository.DeleteAsync(cart);
            await orderRepository.SaveChangesAsync();
        }
    }
}
