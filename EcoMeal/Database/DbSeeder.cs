using EcoMeal.Constants;
using EcoMeal.Entities;
using EcoMeal.Entities.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Database
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var context = services.GetRequiredService<EcoMealDBContext>();

            // 1. Roluri
            foreach (var role in AppRoles.AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // 2. Tabele de lookup (din enum-uri)
            foreach (StatusEnum s in Enum.GetValues<StatusEnum>())
                if (!await context.Statuses.AnyAsync(x => x.Id == s))
                    context.Statuses.Add(new Status { Id = s, Name = s.ToString() });

            foreach (BusinessTypeEnum t in Enum.GetValues<BusinessTypeEnum>())
                if (!await context.BusinessTypes.AnyAsync(x => x.Id == t))
                    context.BusinessTypes.Add(new BusinessType { Id = t, Name = t.ToString() });

            foreach (PackageTypeEnum t in Enum.GetValues<PackageTypeEnum>())
                if (!await context.PackageTypes.AnyAsync(x => x.Id == t))
                    context.PackageTypes.Add(new PackageType { Id = t, Name = t.ToString() });

            await context.SaveChangesAsync();

            // 3. Admin
            var adminEmail = configuration["SeedAdmin:Email"];
            var adminPassword = configuration["SeedAdmin:Password"];

            if (await userManager.FindByEmailAsync(adminEmail) is null)
            {
                var admin = new ApplicationUser
                {
                    FullName = "Admin",
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }

            // 4. Date demo — doar dacă nu există deja business-uri
            if (!await context.Businesses.AnyAsync())
                await SeedSampleDataAsync(userManager, context);
        }

        private static async Task<ApplicationUser> CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string fullName, string email, string password, string role)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
                return existing;

            var user = new ApplicationUser
            {
                FullName = fullName,
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);
            return user;
        }

        private static async Task SeedSampleDataAsync(
            UserManager<ApplicationUser> userManager,
            EcoMealDBContext context)
        {
            // Client de test
            await CreateUserAsync(userManager, "Client Test", "client@ecomeal.com", "Client123!", AppRoles.Customer);

            // Manageri
            var manager1 = await CreateUserAsync(userManager, "Ana Popescu", "ana@brutarie.ro", "Manager123!", AppRoles.BusinessManager);
            var manager2 = await CreateUserAsync(userManager, "Ion Ionescu", "ion@pizzaverde.ro", "Manager123!", AppRoles.BusinessManager);

            // Business-uri (fiecare cu managerul lui)
            var brutarie = new Business
            {
                Id = Guid.NewGuid(),
                Name = "Brutăria Bună",
                Description = "Produse de panificație proaspete, în fiecare zi.",
                Address = "Str. Florilor 12, Arad",
                ImageUrl = "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=400",
                BusinessTypeId = BusinessTypeEnum.Store,
                ManagerId = manager1.Id
            };
            var pizzerie = new Business
            {
                Id = Guid.NewGuid(),
                Name = "Pizza Verde",
                Description = "Pizza artizanală cu ingrediente locale.",
                Address = "Bd. Revoluției 40, Arad",
                ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400",
                BusinessTypeId = BusinessTypeEnum.Restaurant,
                ManagerId = manager2.Id
            };
            context.Businesses.AddRange(brutarie, pizzerie);

            // Pachete
            var now = DateTime.Now;
            context.Packages.AddRange(
                new Package
                {
                    Id = Guid.NewGuid(),
                    Name = "Surpriză de panificație",
                    Description = "Mix de produse rămase la final de zi.",
                    Price = 12.5f,
                    Quantity = 8,
                    PickupStart = now.AddHours(3),
                    PickupEnd = now.AddHours(5),
                    ImageUrl = "https://images.unsplash.com/photo-1608198093002-ad4e005484ec?w=400",
                    PackageTypeId = PackageTypeEnum.Food,
                    BusinessId = brutarie.Id
                },
                new Package
                {
                    Id = Guid.NewGuid(),
                    Name = "Cutie cu cornuri",
                    Description = "Cornuri asortate, proaspete.",
                    Price = 9.0f,
                    Quantity = 5,
                    PickupStart = now.AddHours(2),
                    PickupEnd = now.AddHours(4),
                    ImageUrl = "https://images.unsplash.com/photo-1568254183919-78a4f43a2877?w=400",
                    PackageTypeId = PackageTypeEnum.Food,
                    BusinessId = brutarie.Id
                },
                new Package
                {
                    Id = Guid.NewGuid(),
                    Name = "Pizza box surpriză",
                    Description = "Două pizze medii, alese de bucătar.",
                    Price = 25.0f,
                    Quantity = 4,
                    PickupStart = now.AddHours(1),
                    PickupEnd = now.AddHours(3),
                    ImageUrl = "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400",
                    PackageTypeId = PackageTypeEnum.Food,
                    BusinessId = pizzerie.Id
                }
            );

            await context.SaveChangesAsync();
        }
    }
}