using StudentManagementSystem.Data;
using StudentManagementSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Register services
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

var app = builder.Build();

// Initialize database objects (database, table, stored procedures) at startup.
var dbInitLogger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitializer");
await DatabaseInitializer.InitializeAsync(app.Configuration, dbInitLogger);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
