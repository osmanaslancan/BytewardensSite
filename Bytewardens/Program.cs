using Bytewardens.Data;
using Bytewardens.Handlers;
using Bytewardens.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<IGameService, GameService>();
builder.Services.AddOptions();
builder.Services.Configure<GameApiOptions>(builder.Configuration.GetSection(GameApiOptions.SectionKey));
#if DEBUG
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
#elif RELEASE
var connectionString =  File.ReadAllText("D:\\home\\data\\mysql\\MYSQLCONNSTR_localdb.txt");
connectionString = connectionString.Replace("Data Source", "Server");
connectionString = connectionString.Replace(":", ",");
#endif

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
#if DEBUG
    options.UseSqlServer(connectionString);
#else
    options.UseMySQL(connectionString);
#endif
}
    );
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
}).AddEntityFrameworkStores<ApplicationDbContext>();


var app = builder.Build();


// Migrate latest database changes during startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider
        .GetRequiredService<ApplicationDbContext>();

    // Here is the migration executed
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
