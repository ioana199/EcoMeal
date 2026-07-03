using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.Entities
{
    public class OrderPackage
    {
        public required Guid Id { get; set; }
        public required Guid OrderId { get; set; }
        public required Guid PackageId { get; set; }
        public required int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [ForeignKey("PackageId")]
        public Package Package { get; set; }
    }
}
