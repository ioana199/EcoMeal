namespace EcoMeal.Entities
{
    public class UserWithRole
    {
        public required ApplicationUser User { get; set; }
        public string? Role { get; set; }
        public string? BusinessName { get; set; }
        public Guid? BusinessId { get; set; }
    }
}
