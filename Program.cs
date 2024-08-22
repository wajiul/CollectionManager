using CollectionManager.Data_Access;
using CollectionManager.Data_Access.Entities;
using CollectionManager.Data_Access.Repositories;
using CollectionManager.Hubs;
using CollectionManager.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CollectionMangerDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<User,IdentityRole>().AddEntityFrameworkStores<CollectionMangerDbContext>();
builder.Services.AddScoped<CollectionRepository>();
builder.Services.AddScoped<ItemRepository>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddSignalR();
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
{
    options.ValidationInterval = TimeSpan.Zero;
});

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapHub<ReactionHub>("/hubs/reaction");

app.Run();
