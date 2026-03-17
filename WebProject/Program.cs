using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebProject.DataAccess;
using WebProject.ExternalServices.Implements;
using WebProject.ExternalServices.Interfaces;
using WebProject.Middlewares;
using WebProject.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<WebProjectDbContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));

builder.Services.AddStackExchangeRedisCache(options =>
    options.Configuration = builder.Configuration.GetConnectionString("Redis"));
ThreadPool.SetMinThreads(workerThreads: 10, completionPortThreads: 10);

builder.Services.AddMvc();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<ISessionService, SessionService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Login";
        options.AccessDeniedPath = "/";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
        options.SlidingExpiration = true;

    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WebProjectDbContext>();

    if(!await db.Roles.AnyAsync(r => r.Name == "SuperAdmin"))
    {
        Role role = new()
        {
            Name = "SuperAdmin"
        };

        foreach (int i in Enum.GetValues<Pages>().Select(p => (int)p | (int)PageAccess.Read_Write))
        {
            int a = i;
            //int a = i | (int)Permissions.Read_Write;
            role.Permissions.Add(a);
        }

        await db.Roles.AddAsync(role);
        await db.SaveChangesAsync();
    }

    var hasher = new PasswordHasher<User>();

    if (!await db.Users.AnyAsync(u => u.Username == "admin"))
    {
         await db.Users.AddAsync(new User
        {
            Username = "admin",
            PasswordHash = hasher.HashPassword(null!, "admin"),
            RoleId = "SuperAdmin"
        });
        await db.SaveChangesAsync();
    }
    else if (!await db.Users.AnyAsync(u => u.Username == "admin" && u.RoleId == "SuperAdmin"))
    {
        User user = await db.Users.FindAsync("admin") ?? throw new Exception("admin not found");
        user!.RoleId = "SuperAdmin";
        await db.SaveChangesAsync();
    }
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<SessionValidationMiddleware>();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();