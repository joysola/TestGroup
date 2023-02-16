

using Chapter3.Contracts;
using Chapter3.Service;
using Chapter3.Service.Contracts;
using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using Repository;

namespace Chapter3WebApi.ContextFactory
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services) =>
          services.AddCors(options =>
          {
              options.AddPolicy("CorsPolicy", builder =>
              builder.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
          });

        public static void ConfigureIISIntegration(this IServiceCollection services) =>
            services.Configure<IISOptions>(options =>
            {
            });
        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerManager, LoggerManager>();

        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
            services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<RepositoryContext>(opts => 
            opts.UseMySQL(configuration.GetConnectionString("sqlConnection")));

        public static void ConfigureSqlContext2(this IServiceCollection services, IConfiguration configuration) =>
            services.AddMySQLServer<RepositoryContext>((configuration.GetConnectionString("sqlConnection")));
    }
}
