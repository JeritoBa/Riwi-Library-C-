using Microsoft.EntityFrameworkCore;

using Exercise4.Data;
using Exercise4.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Add services to the container. ---
builder.Services.AddControllersWithViews();

// Custom Services Initializing
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<BooksService>();
builder.Services.AddScoped<LoansService>();

// DB Service Initializing
builder.Services.AddDbContext<MysqlDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    )
);

// Email Service Initializing
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
