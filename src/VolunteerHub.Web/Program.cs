using Microsoft.EntityFrameworkCore;
using VolunteerHub.Infrastructure;
using VolunteerHub.Infrastructure.Persistence;
using VolunteerHub.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Layer services
builder.Services.AddInfrastructure(builder.Configuration);

// Controllers API
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Exception handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Auto migrate database
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await dbContext.Database.MigrateAsync();
}

await VolunteerHub.Infrastructure.Seed.RoleSeeder.SeedAsync(app.Services);
await VolunteerHub.Infrastructure.Persistence.Seeding.BadgeSeeder.SeedAsync(app.Services);
await VolunteerHub.Infrastructure.Persistence.Seeding.NotificationTemplateSeeder.SeedAsync(app.Services);

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();