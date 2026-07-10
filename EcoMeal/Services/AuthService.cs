using EcoMeal.Entities;
using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace EcoMeal.Services
{
    public class AuthService(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : IAuthService
    {
        public async Task<SignInResult> LoginAsync(LoginRequest request)
        {
            return await signInManager.PasswordSignInAsync(
                request.Email, request.Password, true, false);
        }

        public async Task Register(RegisterRequest request)
        {
             
        }

    }
}
