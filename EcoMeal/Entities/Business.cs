using EcoMeal.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EcoMeal.Entities
{
    public class Business
    {
        public Guid Id { get; set; }
        public required BusinessTypeEnum BusinessTypeId { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public required string Address { get; set; }
        public string ImageUrl { get; set; }
        public string? ManagerId { get; set; }
        [ForeignKey("BusinessTypeId")]
        public BusinessType BusinessType { get; set; }
        [ForeignKey("ManagerId")]
        public ApplicationUser? Manager { get; set; }
    }
}
