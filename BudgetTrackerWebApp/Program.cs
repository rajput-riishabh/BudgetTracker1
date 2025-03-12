//using Microsoft.AspNetCore.Authentication.Cookies;
//using BudgetTrackerWebApp.Handlers; 


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Add Cookie Authentication
//    .AddCookie(options =>
//    {
//        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Example: Cookie expiration
//        options.SlidingExpiration = true;
//        options.LoginPath = "/Auth/Login"; // If not authenticated, redirect to Login page
//        options.AccessDeniedPath = "/Auth/AccessDenied"; // Optional: Access Denied page
//    });

//builder.Services.AddControllersWithViews();
//// **Register HttpContextAccessor** - VERY IMPORTANT for DelegatingHandler
//builder.Services.AddHttpContextAccessor();

//// **Register JwtAuthenticationHeaderHandler as a transient service**
//builder.Services.AddTransient<JwtAuthenticationHeaderHandler>();

//// **Register HttpClient**
//builder.Services.AddHttpClient("BudgetTrackerAPI", client =>
//{
//    // Configure the HttpClient to point to your API base address.
//    // **Important:** Replace "Your_API_Base_URL_Here" with the actual base URL of your BudgetTrackerAPI.
//    //client.BaseAddress = new Uri("https://localhost:7053"); // Example: Replace with your API URL (HTTPS)
//    // Or if your API is running on HTTP:
//    client.BaseAddress = new Uri("http://localhost:5285"); // Example: Replace with your API URL (HTTP - if used for testing)

//    // You can add default headers if needed, e.g., for Content-Type
//    client.DefaultRequestHeaders.Accept.Clear();
//    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
//});
//.AddHttpMessageHandler<JwtAuthenticationHeaderHandler>(); // **Ensure this is chained correctly**


//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.Run();



using Microsoft.AspNetCore.Authentication.Cookies;
using BudgetTrackerWebApp.Handlers;
using Microsoft.AspNetCore.Http; // Make sure this is included if not already
using BudgetTrackerWebApp.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Add Cookie Authentication
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60); // Example: Cookie expiration
        options.SlidingExpiration = true;
        options.LoginPath = "/Auth/Login"; // If not authenticated, redirect to Login page
        options.AccessDeniedPath = "/Auth/AccessDenied"; // Optional: Access Denied page
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
    });

builder.Services.AddControllersWithViews();

// **Register HttpContextAccessor** - VERY IMPORTANT for DelegatingHandler
builder.Services.AddHttpContextAccessor();

// **Register JwtAuthenticationHeaderHandler as a transient service**
builder.Services.AddTransient<JwtAuthenticationHeaderHandler>();

// **Register HttpClient with JwtAuthenticationHeaderHandler**
builder.Services.AddHttpClient("BudgetTrackerApiClient") // **EXACT NAME: "BudgetTrackerApiClient"**
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5285");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    })
    .AddHttpMessageHandler<JwtAuthenticationHeaderHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    // **Optional: Disable HTTPS Redirection in Development for easier local testing if needed**
    // app.UseHttpsRedirection(); // Comment out or remove in Development if you are not using HTTPS locally
}


app.UseHttpsRedirection(); // Keep this line if you intend to use HTTPS, even in development if configured.
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // **Authentication middleware MUST come BEFORE Authorization middleware**
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();