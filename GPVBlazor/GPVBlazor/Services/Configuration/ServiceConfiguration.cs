using GPVBlazor.Models;
using GPVBlazor.Services.Interfaces;

namespace GPVBlazor.Services.Configuration
{
    public static class ServiceConfiguration
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var securityOptions = configuration.GetSection("AuthSecurity").Get<AuthSecurityOptions>()
                ?? new AuthSecurityOptions();

            services
                .AddRazorComponents()
                .AddInteractiveServerComponents()
                .AddInteractiveWebAssemblyComponents();
            services.AddCascadingAuthenticationState();
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.Configure<AuthSecurityOptions>(configuration.GetSection("AuthSecurity"));
            services
                .AddAuthentication(GitHubAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                    GitHubAuthenticationDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Cookie.Name = securityOptions.AuthCookieName;
                        options.Cookie.HttpOnly = true;
                        options.Cookie.IsEssential = true;
                        options.Cookie.SameSite = securityOptions.AuthCookieSameSite;
                        options.Cookie.SecurePolicy = securityOptions.AuthCookieSecurePolicy;
                        options.SlidingExpiration = true;
                        options.ExpireTimeSpan = TimeSpan.FromDays(7);
                        options.LoginPath = "/";
                    }
                );
            services.AddAntiforgery(
                options =>
                {
                    options.HeaderName = securityOptions.AntiforgeryHeaderName;
                    options.Cookie.Name = securityOptions.AntiforgeryCookieName;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                    options.Cookie.SameSite = securityOptions.AntiforgeryCookieSameSite;
                    options.Cookie.SecurePolicy = securityOptions.AntiforgeryCookieSecurePolicy;
                }
            );
            services.AddAuthorization();
            services.AddScoped(_ => new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7261/"),
            });
            services.AddSingleton<IGitHubAuthSessionStore, GitHubAuthSessionStore>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<INetworkAnalysisService, NetworkAnalysisService>();
            services.AddScoped<INetworkAnalysisFilterService, NetworkAnalysisFilterService>();
            services.AddScoped<IRepositoryFilterService, RepositoryFilterService>();
            services.AddScoped<IGistFilterService, GistFilterService>();
            services.AddScoped<IAuthService, AuthService>();
        }
    }
}
