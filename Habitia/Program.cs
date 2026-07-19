using Habitia.Data;
using Habitia.Data.Seed;
using Habitia.Enums;
using Habitia.Models;
using Habitia.Services;
using Habitia.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IIncidenciaService, IncidenciaService>();
builder.Services.AddScoped<IMantenimientoService, MantenimientoService>();
builder.Services.AddScoped<ITipoMantenimientoService, TipoMantenimientoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();

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
        "admin@habitia.com";
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
            Nombre = "Admin",
            Apellido = "Sistema",
            Identificacion = "000000000",
            Telefono = "00000000",
            Estado = EstadoUsuarioEnum.Activo,
            FechaRegistro = DateTime.Now
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