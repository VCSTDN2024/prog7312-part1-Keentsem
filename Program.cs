namespace MunicipalServicesApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<MunicipalServicesApp.Data.IssuesRepository>();
builder.Services.AddSingleton<MunicipalServicesApp.Data.UserRepository>();
builder.Services.AddScoped<MunicipalServicesApp.Services.BadgeService>();
builder.Services.AddScoped<MunicipalServicesApp.Services.DataService>();
builder.Services.AddScoped<MunicipalServicesApp.Services.AdvancedDataStructuresService>();
builder.Services.AddScoped<MunicipalServicesApp.Services.PerformanceAnalyzer>();
builder.Services.AddScoped<MunicipalServicesApp.Services.GenericRepository<MunicipalServicesApp.Models.Issue>>();
builder.Services.AddScoped<MunicipalServicesApp.Services.GenericRepository<MunicipalServicesApp.Models.Badge>>();
builder.Services.AddScoped<MunicipalServicesApp.Services.IssueTimelineService>();
builder.Services.AddScoped<MunicipalServicesApp.Services.MunicipalAnalyticsService>();
builder.Services.AddScoped<MunicipalServicesApp.Services.NotificationService>();
builder.Services.AddScoped<MunicipalServicesApp.Services.IssueManagementService>();
builder.Services.AddScoped<MunicipalServicesApp.Services.UserSessionService>();

// Add session support for user login
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
