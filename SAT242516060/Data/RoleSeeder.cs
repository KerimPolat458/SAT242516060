using Microsoft.AspNetCore.Identity;

namespace SAT242516060.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
    {
        // RoleManager servisini çağır
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Akış şemasındaki rollerimiz
        string[] roleNames = { "Admin", "Yonetici", "Ogretmen" };

        foreach (var roleName in roleNames)
        {
            // Rol veritabanında var mı kontrol et
            var roleExist = await roleManager.RoleExistsAsync(roleName);

            // Yoksa oluştur
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}