using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Spa.Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
		var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
		var configuration = new ConfigurationBuilder()
	                       .SetBasePath(Directory.GetCurrentDirectory())
	                       .AddJsonFile("appsettings.json")
	                       .AddJsonFile($"appsettings.{environment}.json", true)
	                       .Build();

		var connectionString = configuration.GetConnectionString("DefaultConnection");

		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}