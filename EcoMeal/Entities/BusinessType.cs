using EcoMeal.Entities.Enums;

namespace EcoMeal.Entities
{
    public class BusinessType
    {
        public required BusinessTypeEnum Id { get; set; }
        public required string Name { get; set; }
    }
}
