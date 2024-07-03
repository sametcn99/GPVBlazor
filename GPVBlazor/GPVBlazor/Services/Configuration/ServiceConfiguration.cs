using global::GPVBlazor.Services.Interfaces;
namespace GPVBlazor.Services.Configuration
{
    public static class ServiceConfiguration
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
            services.AddMemoryCache();
            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7261/") });
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IRepositoryFilterService, RepositoryFilterService>();
        }
    }
}