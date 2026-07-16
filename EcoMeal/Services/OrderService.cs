using EcoMeal.Entities;
using EcoMeal.Entities.Enums;
using EcoMeal.Repositories;
using EcoMeal.Repositories.Interfaces;
using EcoMeal.Services.Interfaces;

namespace EcoMeal.Services
{
    public class OrderService(IOrderRepository orderRepository, IOrderPackageRepository orderPackageRepository, IPackageRepository packageRepository, IBusinessRepository businessRepository) : IOrderService 
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
        public async Task<List<Order>> GetMyOrdersAsync(string userId)
        {
            return await orderRepository.GetByUserWithItemsAsync(userId);
        }

        public async Task ChangeStatusAsync(Guid orderId, StatusEnum newStatus, string callerUserId, bool isAdmin)
        {
            var order = await orderRepository.GetByIdWithItemsAsync(orderId)
                ?? throw new InvalidOperationException("Comanda nu există.");

            // Ownership: managerul poate atinge doar comenzile business-ului lui
            if (!isAdmin)
            {
                var business = await businessRepository.GetByManagerAsync(callerUserId)
                    ?? throw new InvalidOperationException("Nu administrezi niciun business.");

                if (order.BusinessId != business.Id)
                    throw new InvalidOperationException("Nu poți modifica o comandă a altui business.");
            }

            var current = order.StatusId;

            // Tranziția e permisă?
            if (!IsTransitionAllowed(current, newStatus))
                throw new InvalidOperationException($"Tranziția din {current} în {newStatus} nu e permisă.");

            // Efecte pe stoc
            if (current == StatusEnum.New && newStatus == StatusEnum.Reserved)
            {
                // reverifică stocul înainte să scazi
                foreach (var line in order.OrderPackages)
                {
                    if (line.Package.Quantity < line.Quantity)
                        throw new InvalidOperationException(
                            $"Stoc insuficient pentru {line.Package.Name} (disponibil: {line.Package.Quantity}).");
                }

                foreach (var line in order.OrderPackages)
                    line.Package.Quantity -= line.Quantity;

                // număr secvențial la plasare
                var nextNumber = await orderRepository.GetNextOrderNumberAsync();
                order.OrderNumber = nextNumber.ToString();
            }
            else if (current == StatusEnum.Reserved && newStatus == StatusEnum.Cancelled)
            {
                // dă stocul înapoi
                foreach (var line in order.OrderPackages)
                    line.Package.Quantity += line.Quantity;
            }
            // New → Cancelled: nimic pe stoc (coșul n-a scăzut nimic)
            // Reserved → PickedUp, PickedUp → Reviewed: nimic pe stoc

            order.StatusId = newStatus;
            await orderRepository.SaveChangesAsync();
        }

        private static bool IsTransitionAllowed(StatusEnum current, StatusEnum next)
        {
            return current switch
            {
                StatusEnum.New => next is StatusEnum.Reserved or StatusEnum.Cancelled,
                StatusEnum.Reserved => next is StatusEnum.PickedUp or StatusEnum.Cancelled,
                StatusEnum.PickedUp => next is StatusEnum.Reviewed,
                _ => false   // Reviewed și Cancelled sunt finale
            };
        }

        public async Task DeleteOrderAsync(Guid orderId, string callerUserId, bool isAdmin)
        {
            var order = await orderRepository.GetByIdWithItemsAsync(orderId)
                ?? throw new InvalidOperationException("Comanda nu există.");

            if (!isAdmin)
            {
                var business = await businessRepository.GetByManagerAsync(callerUserId)
                    ?? throw new InvalidOperationException("Nu administrezi niciun business.");

                if (order.BusinessId != business.Id)
                    throw new InvalidOperationException("Nu poți șterge o comandă a altui business.");
            }

            // Doar comenzile anulate pot fi șterse
            if (order.StatusId != StatusEnum.Cancelled)
                throw new InvalidOperationException("Doar comenzile anulate pot fi șterse.");

            foreach (var line in order.OrderPackages.ToList())
                await orderPackageRepository.DeleteAsync(line);

            await orderRepository.DeleteAsync(order);
            await orderRepository.SaveChangesAsync();
        }

        public async Task<List<Order>> GetOrdersForManagementAsync(string callerUserId, bool isAdmin)
        {
            if (isAdmin)
                return await orderRepository.GetAllWithItemsAsync();

            // manager: doar comenzile business-ului lui
            var business = await businessRepository.GetByManagerAsync(callerUserId);
            if (business is null)
                return new List<Order>();

            return await orderRepository.GetByBusinessWithItemsAsync(business.Id);
        }

        public IEnumerable<StatusEnum> GetAllowedTransitions(StatusEnum current)
        {
            return current switch
            {
                StatusEnum.New => new[] { StatusEnum.Reserved, StatusEnum.Cancelled },
                StatusEnum.Reserved => new[] { StatusEnum.PickedUp, StatusEnum.Cancelled },
                StatusEnum.PickedUp => new[] { StatusEnum.Reviewed },
                _ => Array.Empty<StatusEnum>()
            };
        }
    }
}
