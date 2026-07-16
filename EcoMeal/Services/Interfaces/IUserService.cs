using EcoMeal.Entities;

namespace EcoMeal.Services.Interfaces
{
    public interface IUserService
    {
        public Task<List<UserWithRole>> GetAllAsync();
        public Task ChangeRoleAsync(string userId, string newRole, Guid? businessId);
        public Task<(bool Success, string? Error)> DeleteAsync(string userId);
        public Task<(bool Success, string? Error)> CreateAsync(string fullName, string email, string password, string role, Guid? businessId);
    }
}
