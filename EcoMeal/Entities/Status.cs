using EcoMeal.Entities.Enums;

namespace EcoMeal.Entities
{
    public class Status
    {
        public required StatusEnum Id { get; set; }
        public required string Name { get; set; }
    }
}
