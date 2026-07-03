using EcoMeal.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.Entities
{
    public class Package
    {
        public required Guid Id { get; set; }
        public required Guid BusinessId { get; set; }
        public required PackageTypeEnum PackageTypeId { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public required float Price { get; set; }
        public required int Quantity { get; set; }
        public required DateTime PickupStart { get; set; }
        public required DateTime PickupEnd { get; set; }
        public string ImageUrl { get; set; }
        [ForeignKey("BusinessId")]
        public Business Business { get; set; }
        [ForeignKey("PackageTypeId")]
        public PackageType PackageType { get; set; }

    }
}
