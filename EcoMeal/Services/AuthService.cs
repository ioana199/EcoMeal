using EcoMeal.Constants;
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

        public async Task<IdentityResult> RegisterAsync(RegisterModel request)
        {
           var existingUser = await userManager.FindByEmailAsync(request.Email);

            if(existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { });
            }
         
           var newUser = new ApplicationUser
           { 
               UserName=request.Email,
               FullName = request.FullName,
               Email = request.Email,
           };
            var userToCreate = await userManager.CreateAsync(newUser, request.Password);
            if (!userToCreate.Succeeded)
                return userToCreate;

            await userManager.AddToRoleAsync(newUser, AppRoles.Customer);
            return userToCreate;
        }

        public async Task LogoutAsync()
        {
            await signInManager.SignOutAsync();
        }
    }
}
