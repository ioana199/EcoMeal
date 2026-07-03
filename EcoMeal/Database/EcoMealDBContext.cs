using EcoMeal.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Database
{
    public class EcoMealDBContext : IdentityDbContext<ApplicationUser>
    {
        public EcoMealDBContext(DbContextOptions<EcoMealDBContext> options) : base(options)
        {
            
        }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<BusinessType> BusinessTypes
        {
            get; set;
        }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<OrderPackage> OrderPackages
        {
            get; set;
        }
        public DbSet<Package> Packages
        {
            get; set;
        }
        public DbSet<PackageType> PackageTypes
        {
            get; set;
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
    
        }


    }
}

