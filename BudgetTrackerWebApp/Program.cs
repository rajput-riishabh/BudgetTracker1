var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); 

// **Register HttpClient**
builder.Services.AddHttpClient("BudgetTrackerAPI", client =>
{
    // Configure the HttpClient to point to your API base address.
    // **Important:** Replace "Your_API_Base_URL_Here" with the actual base URL of your BudgetTrackerAPI.
    //client.BaseAddress = new Uri("https://localhost:7053"); // Example: Replace with your API URL (HTTPS)
    // Or if your API is running on HTTP:
    client.BaseAddress = new Uri("http://localhost:5285"); // Example: Replace with your API URL (HTTP - if used for testing)

    // You can add default headers if needed, e.g., for Content-Type
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
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

app.Run();
