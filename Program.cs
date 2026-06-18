using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Interfaces;
using Zariya.Repositories;
using Zariya.Services;
using Zariya.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ZariyaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register app services so Razor Pages stay thin and data access stays centralized.
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDonorRepository, DonorRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IReferenceDataService, ReferenceDataService>();
builder.Services.AddScoped<IDonationTypeService, DonationTypeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDonorService, DonorService>();
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IRequestService, RequestService>();
builder.Services.AddScoped<RegisterDonorValidator>();

// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var donationTypeService = scope.ServiceProvider.GetRequiredService<IDonationTypeService>();
    try
    {
        await donationTypeService.EnsureDefaultDonationTypesAsync();
    }
    catch
    {
        // The request path still reports database errors; startup should remain usable when SQL Server is offline.
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Support/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
