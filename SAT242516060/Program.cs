using DbContexts;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Providers;
using SAT242516060.Components;
using SAT242516060.Components.Account;
using SAT242516060.Data;
using UnitOfWorks;
using SAT242516060.Services; // LoggerService için
using System.Globalization;

using QuestPDF.Infrastructure; // Madde 24: Raporlama (Lazým olunca açarsýn)

var builder = WebApplication.CreateBuilder(args);

// Madde 24: QuestPDF Community Lisansý ayarý
QuestPDF.Settings.License = LicenseType.Community;

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// Madde 17: Kimlik Doðrulama Ayarlarý
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 1. Standart Identity Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Senin Özel Mimarindeki Context (Madde 22)
builder.Services.AddDbContext<MyDbModel_DbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Madde 18: Rol Bazlý Yetkilendirme
builder.Services.AddIdentityCore<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    // Þifre kurallarý
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
})
    .AddRoles<IdentityRole>() // ROL SERVÝSÝ
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// -----------------------------------------------------------------------------
// SENÝN ÖZEL MÝMARÝ SERVÝSLERÝN (DI REGISTRATION)
// Madde 22: DB -> DbContext -> UnitOfWork -> Provider -> Component hiyerarþisi
// -----------------------------------------------------------------------------
builder.Services.AddScoped<IMyDbModel_UnitOfWork, MyDbModel_UnitOfWork<MyDbModel_DbContext>>();
builder.Services.AddScoped<IMyDbModel_Provider, MyDbModel_Provider>();

// Madde 20: Localization (Yerelleþtirme) Servisleri
// ÖNEMLÝ: ResourcesPath parametresi eklendi
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddControllers();

// Madde 19: Loglama Servisleri
builder.Services.AddScoped<DbLogger>();
builder.Services.AddScoped<FileLogger>();
builder.Services.AddScoped<PdfReportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

// Madde 20: Dil Ayarlarý Middleware
var supportedCultures = new[] { "tr-TR", "en-US" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("tr-TR")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// --- ROL OLUÞTURMA BLOÐU (Madde 18) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Rolleri veritabanýna iþle
        await RoleSeeder.SeedRolesAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Roller oluþturulurken bir hata meydana geldi.");
    }
}

app.MapPost("/Logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Redirect("/Account/Login");
});

// --- ROL VE KULLANICI OLUÞTURMA BLOÐU ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // 1. Önce Rolleri oluþtur
        await RoleSeeder.SeedRolesAsync(services);

        // 2. Sonra Admin kullanýcýsýný oluþtur (YENÝ EKLENEN)
        await UserSeeder.SeedUserAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabaný baþlangýç verileri oluþturulurken hata çýktý.");
    }
}

app.Run();