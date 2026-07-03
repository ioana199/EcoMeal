using EcoMeal.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
        public required StatusEnum StatusId { get; set; }
        public required Guid BusinessId { get; set; }
        public required string OrderNumber { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
        [ForeignKey("StatusId")]
        public Status Status { get; set; }
        public Business Business { get; set; }
    }
}
