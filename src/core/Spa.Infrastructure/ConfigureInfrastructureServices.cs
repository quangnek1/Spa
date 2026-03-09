using Microsoft.Extensions.DependencyInjection;

namespace Spa.Infrastructure
{
	public static class ConfigureInfrastructureServices
	{
		//public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		//{
		//	services.AddDbContext<OrderContext>(options =>
		//	{
		//		options.UseSqlServer(configuration.GetConnectionString(name: "DefaultConnectionString"),
		//		builder => builder.MigrationsAssembly(typeof(OrderContext).Assembly.FullName));
		//	});

		//	services.AddScoped<OrderContextSeed>();
		//	services.AddScoped<IOrderRepository, OrderRepository>();
		//	services.AddScoped(serviceType: typeof(IUnitOfWork<>), implementationType: typeof(UnitOfWork<>));

		//	services.AddScoped(serviceType: typeof(ISmtpEmailService), implementationType: typeof(SmtpEmailService));
		//	//	services.AddAutoMapper(cfg => cfg.AddProfile(new MappingProfile()));
		//	return services;
		//}
	}
}
