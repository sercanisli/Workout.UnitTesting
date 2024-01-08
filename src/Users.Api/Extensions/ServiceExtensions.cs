using Microsoft.EntityFrameworkCore;
using Users.Api.Context;
using Users.Api.Logging;
using Users.Api.Models;
using Users.Api.Repositories;
using Users.Api.Services;

namespace Users.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlConnection(this IServiceCollection services, IConfiguration configuration) 
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
        }

        public static void ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserManager>();
        }

    }
}
