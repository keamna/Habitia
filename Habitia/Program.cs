using Habitia.Data;
using Habitia.Data.Seed;
using Habitia.Models;
using Habitia.Services;
using Habitia.Services.Interfaces;
using Microsoft.AspNetCore.Builder;           
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);  

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<ICargoVencimientoService, CargoVencimientoService>();

builder.Services.AddHangfire(config => config
    .UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IIncidenciaService, IncidenciaService>();
builder.Services.AddScoped<IMantenimientoService, MantenimientoService>();
builder.Services.AddScoped<ITipoMantenimientoService, TipoMantenimientoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<ICargoRecurrenteService, CargoRecurrenteService>();

// =====================================================
// ⭐ CONFIGURACIÓN DEL EMAIL SENDER - BREVO
// =====================================================
builder.Services.AddScoped<IEmailSender>(provider =>
{
    var brevoApiKey = builder.Configuration["Email:BrevoApiKey"];
    var logger = provider.GetRequiredService<ILogger<BrevoEmailSender>>();

    if (!string.IsNullOrEmpty(brevoApiKey))
    {
        return new BrevoEmailSender(brevoApiKey, logger);
    }

    // Fallback a Gmail (si no está configurado Brevo)
    var smtpHost = builder.Configuration["Email:SmtpHost"] ?? "smtp.gmail.com";
    var smtpPort = int.Parse(builder.Configuration["Email:SmtpPort"] ?? "587");
    var smtpUsername = builder.Configuration["Email:SmtpUsername"];
    var smtpPassword = builder.Configuration["Email:SmtpPassword"];
    var enableSSL = bool.Parse(builder.Configuration["Email:EnableSSL"] ?? "true");

    if (string.IsNullOrEmpty(smtpUsername) || string.IsNullOrEmpty(smtpPassword))
    {
        throw new InvalidOperationException(
            "Ni Brevo ni SMTP están configurados correctamente."
        );
    }

    return new EmailSender(smtpHost, smtpPort, enableSSL, smtpUsername, smtpPassword);
});
// =====================================================

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(
        Path.Combine(builder.Environment.ContentRootPath, "DataProtection-Keys")))
    .SetApplicationName("Habitia");

var app = builder.Build();

// Crear roles, administrador y catálogos base
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();
    var context =
        services.GetRequiredService<ApplicationDbContext>();

    string[] roles =
    {
        "Admin",
        "Residente",
        "Seguridad",
        "Mantenimiento"
    };

    foreach (var rol in roles)
    {
        if (!await roleManager.RoleExistsAsync(rol))
        {
            await roleManager.CreateAsync(
                new IdentityRole(rol));
        }
    }

    string email =
        "keanymenaberrocal13@gmail.com";
    string password =
        "Admin@12345";

    var admin =
        await userManager.FindByEmailAsync(email);

    if (admin == null)
    {
        admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            TC_Nombre = "Admin 2FA",
            TC_Apellido = "Sistema",
            TC_Identificacion = "000000000",
            TC_Telefono = "00000000",
            TN_Estado = EstadoUsuarioEnum.Activo,
            TF_FechaRegistro = DateTime.Now
        };

        var result =
            await userManager.CreateAsync(
                admin,
                password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                admin,
                "Admin");

            await userManager.SetTwoFactorEnabledAsync(
                admin,
                true);
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(admin, "Admin"))
        {
            await userManager.AddToRoleAsync(
                admin,
                "Admin");
        }

        if (!admin.TwoFactorEnabled)
        {
            await userManager.SetTwoFactorEnabledAsync(
                admin,
                true);
        }
    }

    // Seed de catálogos
    DbSeeder.SeedTiposArea(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<ICargoVencimientoService>(
    "revisar-cargos-vencidos",
    service => service.ProcesarCargosVencidosAsync(),
    Cron.Daily); // corre todos los días a medianoche

RecurringJob.AddOrUpdate<ICargoRecurrenteService>(
    "generar-cargos-recurrentes",
    service => service.GenerarCargosPendientesAsync(),
    Cron.Daily);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rutas directas
app.MapControllerRoute(
    name: "admin_default",
    pattern: "Admin",
    defaults: new
    {
        area = "Admin",
        controller = "Dashboard",
        action = "Index"
    });

app.MapControllerRoute(
    name: "residente_default",
    pattern: "Residente",
    defaults: new
    {
        area = "Residente",
        controller = "Home",
        action = "Index"
    });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();