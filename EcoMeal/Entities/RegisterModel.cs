using Microsoft.AspNetCore.Identity;

namespace EcoMeal.Entities
{
    public class RegisterModel : IdentityUser
    {
        public required string FullName { get; set; }
    }
}
