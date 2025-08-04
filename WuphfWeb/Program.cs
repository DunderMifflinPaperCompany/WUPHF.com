using Microsoft.EntityFrameworkCore;
using WuphfWeb.Data;
using WuphfWeb.Hubs;
using WuphfWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

// Add Entity Framework with In-Memory database
builder.Services.AddDbContext<WuphfDbContext>(options =>
    options.UseInMemoryDatabase("WuphfDb"));

// Add custom services
builder.Services.AddScoped<IWuphfService, WuphfService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddSingleton<IPrinterService, PrinterService>();
builder.Services.AddSingleton<ISoundService, SoundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.MapHub<WuphfHub>("/wuphfhub");

// Seed demo data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WuphfDbContext>();
    var seeder = new DataSeeder(context);
    await seeder.SeedAsync();
}

app.Run();
