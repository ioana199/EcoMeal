using EcoMeal.Entities.Enums;

namespace EcoMeal.Entities
{
    public class PackageType
    {
        public required PackageTypeEnum Id { get; set; }
        public required string Name { get; set; }
    }
}
