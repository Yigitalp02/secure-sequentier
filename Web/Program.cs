using System;
using Web.Options;
using Web.Services;


var builder = WebApplication.CreateBuilder(args);

// MVC + Razor
builder.Services.AddControllersWithViews();

// In-memory session storage
builder.Services.AddDistributedMemoryCache();


builder.Services.Configure<WebPathsOptions>(opt =>
{
    builder.Configuration.GetSection("SecureSequential").Bind(opt);
    // Allow override via environment variable for Docker deployment
    var envConfig = Environment.GetEnvironmentVariable("DEFAULT_CONFIG_PATH");
    if (!string.IsNullOrEmpty(envConfig))
        opt.DefaultConfig = envConfig;
});

builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
});

// Typed client to SecureSolution2 backend
var backendUrl = Environment.GetEnvironmentVariable("BACKEND_API_URL") ?? "http://localhost:9999/";
builder.Services.AddHttpClient<ISecureSequentialApi, SecureSequentialApi>(http =>
{
    http.BaseAddress = new Uri(backendUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();          // ← enable session BEFORE routing
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FileQueue}/{action=Index}/{id?}");

app.Run();
