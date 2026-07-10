using EcoMeal.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

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
    }
}
