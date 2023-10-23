using WebAPI.Data;
using WebAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Initialize database tables
DBManager.CreateClientTable();
DBManager.CreateJobTable();

// Seed data (optional)
DBManager.InsertClient(new Client { IPAddress = "192.168.1.1", Port = 8080 });
DBManager.InsertClient(new Client { IPAddress = "192.168.1.2", Port = 8081 });

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
