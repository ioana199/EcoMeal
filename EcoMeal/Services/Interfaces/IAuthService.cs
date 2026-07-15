using EcoMeal.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace EcoMeal.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<SignInResult> LoginAsync(LoginRequest request);
        public Task<IdentityResult> RegisterAsync(RegisterModel request);
        public Task LogoutAsync();

    }
}
