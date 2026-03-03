using Radzen;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using CHKS.Data;
using Microsoft.AspNetCore.Identity;
using CHKS.Models;
using MudBlazor.Services;
using CHKS.Services;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddServerSideBlazor().AddHubOptions(o =>
{
    o.MaximumReceiveMessageSize = 10 * 1024 * 1024;
});

builder.Services.AddRazorPages();

builder.Services
    .AddMudServices()
    .AddScoped<InventoryNotificationHubConnectionService>()
    .AddScoped<InventoryNotificationHub>()
    .AddScoped<InventoryControlService>()
    .AddScoped<CartControlService>()
    .AddScoped<StockLogsTrackingService>()
    .AddScoped<EmployeeControl>()
    .AddScoped<SecurityService>()
    .AddTransient<VehicleAPI>()
    .AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
    });

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});


/* REGISTER DATABASE CONTEXTS */
var connectionString = builder.Configuration.GetConnectionString("development");
var isTestingMode = builder.Configuration.GetValue<bool>("TestingMode:Enabled");

if (isTestingMode)
{
    builder.Services
        .AddDbContextFactory<Rardi_Context>(options =>
        {
            options
                .UseInMemoryDatabase("Rardi_Testing_Mode")
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        })
        .AddDbContext<ApplicationIdentityDbContext>(options =>
        {
            options
                .UseInMemoryDatabase("Rardi_Identity_Testing_Mode")
                .ConfigureWarnings(warnings => warnings.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });
}
else
{
    builder.Services
        .AddDbContextFactory<Rardi_Context>(
            options =>
            {
                options.UseMySql(
                        connectionString,
                        ServerVersion.AutoDetect(connectionString)
                );
            }
        ).AddDbContext<ApplicationIdentityDbContext>(
            options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString)
                );
            });
}

/* REGISTER CLIENT HTTPS */
builder.Services
    .AddHttpClient("CHKS")
    .ConfigurePrimaryHttpMessageHandler(
        () => new HttpClientHandler { UseCookies = true }) // Changed to true to enable cookie handling
            .AddHeaderPropagation(o => o.Headers.Add("Cookie")
    );


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.LoginPath = "/Login"; // Specify login path
    options.LogoutPath = "/Account/Logout"; // Specify logout path
    options.AccessDeniedPath = "/Unauthorized"; // Specify access denied path
    options.ExpireTimeSpan = TimeSpan.FromHours(24); // Set cookie expiration
    options.SlidingExpiration = true; // Renew cookie on activity
});

/* SECURITY SERVICES */

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddScoped<SecurityService>()
                .AddScoped<ServerAuthenticationStateProvider>()
                .AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
                .AddDefaultTokenProviders();

builder.Services.AddCors(
    options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
    }
);

/* BUILDING */
var app = builder.Build();

if (isTestingMode)
{
    using var scope = app.Services.CreateScope();
    await TestingModeSeeder.SeedAsync(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseHeaderPropagation();
app.UseStaticFiles();
app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapBlazorHub(); 
app.MapHub<InventoryNotificationHub>("/inventorylogs");
app.MapFallbackToPage("/_Host");

app.Run();