using EcoMeal.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

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
             builder.Entity<OrderPackage>()
        .HasOne(op => op.Package)
        .WithMany()
        .HasForeignKey(op => op.PackageId)
        .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Business>()
    .HasOne(b => b.Manager)
    .WithOne(u => u.ManagedBusiness)
    .HasForeignKey<Business>(b => b.ManagerId)
    .OnDelete(DeleteBehavior.SetNull);
        }


    }
}

