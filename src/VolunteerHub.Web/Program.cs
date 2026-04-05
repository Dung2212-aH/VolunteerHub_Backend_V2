using VolunteerHub.Infrastructure;
using VolunteerHub.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// ── Layer services ──────────────────────────────────────────────────
builder.Services.AddInfrastructure(builder.Configuration);

// ── MVC ─────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// ── Exception handling ──────────────────────────────────────────────
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Seed identity roles
await VolunteerHub.Infrastructure.Seed.RoleSeeder.SeedAsync(app.Services);

// Seed badges and notification templates
await VolunteerHub.Infrastructure.Persistence.Seeding.BadgeSeeder.SeedAsync(app.Services);
await VolunteerHub.Infrastructure.Persistence.Seeding.NotificationTemplateSeeder.SeedAsync(app.Services);

// ── Pipeline ────────────────────────────────────────────────────────
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
