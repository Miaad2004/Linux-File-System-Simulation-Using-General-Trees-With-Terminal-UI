using FileSystem.Data;
using FileSystem.Services.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileSystem
{
    public static class ServiceFactory
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<LinuxDbContext>();
            services.AddScoped<IPasswordService, PasswordServiceSHA>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddSingleton(configuration);
        }
    }
}
