using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using EcoMeal.Entities;

namespace EcoMeal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginRequest request, [FromQuery] string? returnUrl)
        {
            var result = await authService.LoginAsync(request);
            if(result.Succeeded)
                return LocalRedirect(returnUrl ?? "/");

            return LocalRedirect($"/account/login?error=Invalid login&returnUrl={returnUrl}");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterModel request, [FromQuery] string? returnUrl)
        {
            var result = await authService.RegisterAsync(request);
            if(result.Succeeded)
                return LocalRedirect(returnUrl ?? "/");
            
            return LocalRedirect($"/account/register?error=Invalid register&returnUrl={returnUrl}");
        }
    }
}
