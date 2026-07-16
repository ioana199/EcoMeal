using EcoMeal.Constants;
using EcoMeal.Entities;
using EcoMeal.Repositories.Interfaces;
using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Services
{
    public class UserService(
        UserManager<ApplicationUser> userManager,
        IBusinessRepository businessRepository,
        IOrderRepository orderRepository) : IUserService
    {
        public async Task<List<UserWithRole>> GetAllAsync()
        {
            var users = await userManager.Users.ToListAsync();
            var businesses = await businessRepository.GetAllAsync();

            var result = new List<UserWithRole>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                var managedBusiness = businesses.FirstOrDefault(b => b.ManagerId == user.Id);

                result.Add(new UserWithRole
                {
                    User = user,
                    Role = roles.FirstOrDefault(),
                    BusinessName = managedBusiness?.Name,
                    BusinessId = managedBusiness?.Id,
                });
            }
            return result;
        }

        public async Task ChangeRoleAsync(string userId, string newRole, Guid? businessId)
        {
            var user = await userManager.FindByIdAsync(userId)
                ?? throw new InvalidOperationException("Userul nu există.");

            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);

            var previouslyManaged = (await businessRepository.GetAllAsync())
                .FirstOrDefault(b => b.ManagerId == user.Id);
            if (previouslyManaged is not null)
            {
                previouslyManaged.ManagerId = null;
            }

            await userManager.AddToRoleAsync(user, newRole);

            if (newRole == AppRoles.BusinessManager)
            {
                if (businessId is null)
                    throw new InvalidOperationException("Alege un business pentru manager.");

                var business = await businessRepository.GetByIdAsync(businessId.Value)
                    ?? throw new InvalidOperationException("Business-ul nu există.");

                if (business.ManagerId is not null)
                    throw new InvalidOperationException("Business-ul are deja un manager.");

                business.ManagerId = user.Id;
            }

            await businessRepository.SaveChangesAsync();
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null)
                return (false, "Userul nu există.");

            var orders = await orderRepository.GetAllAsync();
            if (orders.Any(o => o.UserId == userId))
                return (false, "Userul nu poate fi șters pentru că are comenzi.");

            var managedBusiness = (await businessRepository.GetAllAsync())
                .FirstOrDefault(b => b.ManagerId == userId);
            if (managedBusiness is not null)
            {
                managedBusiness.ManagerId = null;
                await businessRepository.SaveChangesAsync();
            }

            await userManager.DeleteAsync(user);
            return (true, null);
        }

        public async Task<(bool Success, string? Error)> CreateAsync(
    string fullName, string email, string password, string role, Guid? businessId)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
                return (false, "Există deja un cont cu acest email.");

            Business? business = null;
            if (role == AppRoles.BusinessManager)
            {
                if (businessId is null)
                    return (false, "Alege un business pentru manager.");

                business = await businessRepository.GetByIdAsync(businessId.Value);
                if (business is null)
                    return (false, "Business-ul nu există.");

                if (business.ManagerId is not null)
                    return (false, "Business-ul are deja un manager.");
            }

             
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, string.Join(" ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, role);

            if (business is not null)
            {
                business.ManagerId = user.Id;
                await businessRepository.SaveChangesAsync();
            }

            return (true, null);
        }
    }
}