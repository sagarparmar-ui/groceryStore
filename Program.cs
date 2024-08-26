using Microsoft.AspNetCore.Authentication.Cookies;
using RMS.Services.CurrentUserService;
using RMS.Services.ProductService;
using Microsoft.EntityFrameworkCore;
using RMS.Services.ShoppingCartService;
using RMS.Services.UserService;
using RMS.GenericRepository;
using RMS.Models;
using RMS.Data;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GroceryStoreContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("GroceryStoreDB") ??
                                            throw new InvalidOperationException("Connection string 'GroceryStoreDB' not found.")));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUserService, UserService>(); 
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(option =>
                    {
                        option.ExpireTimeSpan = TimeSpan.FromMinutes(60 * 1);
                        option.LoginPath = "/Users/Login";
                        option.AccessDeniedPath = "/Users/AccessDenied";
                    });

builder.Services.AddSession(option =>
                {
                    option.IdleTimeout = TimeSpan.FromMinutes(15);
                    option.Cookie.HttpOnly = true;
                    option.Cookie.IsEssential = true;
                });


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

//Initialize services in the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
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
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.Run();
