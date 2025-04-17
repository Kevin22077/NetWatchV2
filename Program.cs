using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using NetWatchV2.Data;
using Microsoft.AspNetCore.Authentication;
using NetWatchV2.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//Add the database context
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(SessionAuthSchemeConstants.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, SessionAuthHandler>(
        SessionAuthSchemeConstants.SchemeName, options => { });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de inactividad antes de que la sesión expire
    options.Cookie.HttpOnly = true; // Protección contra JavaScript del lado del cliente
    options.Cookie.IsEssential = true; // Indica que esta cookie es esencial para la funcionalidad
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.Requirements.Add(new AdminRequirement()));
});

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var enUSCulture = new CultureInfo("en-US");
    options.DefaultRequestCulture = new RequestCulture(enUSCulture);
    options.SupportedCultures = new[] { enUSCulture };
    options.SupportedUICultures = new[] { enUSCulture };
});

builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Asegúrate de que la base de datos se haya creado y las migraciones aplicadas
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
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
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.UseRequestLocalization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();