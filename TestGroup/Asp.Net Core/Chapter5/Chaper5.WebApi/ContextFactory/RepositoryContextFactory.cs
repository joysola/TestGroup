using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Repository;

namespace Chaper5.WebApi
{
    public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
    {
        public RepositoryContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<RepositoryContext>()
                .UseMySQL(configuration.GetConnectionString("sqlConnection"),
                b => b.MigrationsAssembly("Chaper5.WebApi"));
            return new RepositoryContext(builder.Options);
        }
    }
}
