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

        // Descrierea unui business de semănat (date + managerul lui)
        private record BusinessSeed(
            string Name, string Description, string Address, string ImageUrl,
            BusinessTypeEnum Type, PackageTypeEnum PackageType,
            string ManagerName, string ManagerEmail);

        private static async Task SeedSampleDataAsync(
            UserManager<ApplicationUser> userManager,
            EcoMealDBContext context)
        {
            var seeds = new List<BusinessSeed>
            {
                new("Brutăria Bună", "Produse de panificație proaspete, în fiecare zi.",
                    "Str. Florilor 12, Arad", "https://images.unsplash.com/photo-1509440159596-0249088772ff?w=400",
                    BusinessTypeEnum.Store, PackageTypeEnum.Food,
                    "Ana Popescu", "ana@brutarie.ro"),

                new("Pizza Verde", "Pizza artizanală cu ingrediente locale.",
                    "Bd. Revoluției 40, Arad", "https://images.unsplash.com/photo-1513104890138-7c749659a591?w=400",
                    BusinessTypeEnum.Restaurant, PackageTypeEnum.Food,
                    "Ion Ionescu", "ion@pizzaverde.ro"),

                new("Cofetăria Dulce", "Prăjituri și torturi făcute în casă.",
                    "Str. Mucius Scaevola 5, Arad", "https://images.unsplash.com/photo-1486427944299-d1955d23e34d?w=400",
                    BusinessTypeEnum.Store, PackageTypeEnum.Food,
                    "Maria Georgescu", "maria@cofetarie.ro"),

                new("Bistro Verde", "Preparate calde, gătite zilnic.",
                    "Str. Episcopiei 8, Arad", "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?w=400",
                    BusinessTypeEnum.Restaurant, PackageTypeEnum.Food,
                    "Andrei Marin", "andrei@bistroverde.ro"),

                new("Cafeneaua Boabă", "Cafea de specialitate și băuturi.",
                    "Str. Unirii 22, Arad", "https://images.unsplash.com/photo-1501339847302-ac426a4a7cbb?w=400",
                    BusinessTypeEnum.Store, PackageTypeEnum.Drink,
                    "Elena Stan", "elena@boaba.ro"),

                new("Farmacia Sănătatea", "Produse și suplimente aproape de expirare, la preț redus.",
                    "Str. Vasile Goldiș 3, Arad", "https://images.unsplash.com/photo-1587854692152-cbe660dbde88?w=400",
                    BusinessTypeEnum.Pharmacy, PackageTypeEnum.Medicine,
                    "Radu Dima", "radu@sanatatea.ro"),
            };

            var now = DateTime.Now;
            var random = new Random();

            foreach (var seed in seeds)
            {
                // managerul business-ului
                var manager = await CreateUserAsync(
                    userManager, seed.ManagerName, seed.ManagerEmail, "Manager123!", AppRoles.BusinessManager);

                // business-ul
                var business = new Business
                {
                    Id = Guid.NewGuid(),
                    Name = seed.Name,
                    Description = seed.Description,
                    Address = seed.Address,
                    ImageUrl = seed.ImageUrl,
                    BusinessTypeId = seed.Type,
                    ManagerId = manager.Id
                };
                context.Businesses.Add(business);

                // 5 pachete pentru business
                for (int i = 1; i <= 5; i++)
                {
                    var startOffset = random.Next(1, 6); // peste 1-5 ore
                    context.Packages.Add(new Package
                    {
                        Id = Guid.NewGuid(),
                        Name = $"{seed.Name} — pachet #{i}",
                        Description = "Pachet-surpriză cu produse rămase, la preț redus.",
                        Price = (float)Math.Round(random.NextDouble() * 25 + 5, 2), // 5–30 lei
                        Quantity = random.Next(3, 12),                              // 3–11 buc
                        PickupStart = now.AddHours(startOffset),
                        PickupEnd = now.AddHours(startOffset + 2),
                        ImageUrl = seed.ImageUrl,
                        PackageTypeId = seed.PackageType,
                        BusinessId = business.Id
                    });
                }
            }

            await context.SaveChangesAsync();
        }
    }
}