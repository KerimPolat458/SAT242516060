using Microsoft.AspNetCore.Identity;

namespace SAT242516060.Data;

public static class UserSeeder
{
    public static async Task SeedUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Varsayılan Admin Kullanıcısı
        var adminEmail = "admin@okul.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true // Email onayıyla uğraşmamak için
            };

            // Şifre: "123" (Program.cs'de kuralları esnetmiştik)
            await userManager.CreateAsync(adminUser, "123");

            // Admin rolünü ata
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}