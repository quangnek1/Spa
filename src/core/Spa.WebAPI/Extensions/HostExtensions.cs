namespace Spa.WebAPI.Extensions
{
	public static class HostExtensions
	{
		public static void AddAppConfigurations(this ConfigureHostBuilder host)
		{
			host.ConfigureAppConfiguration((context, config) =>
			{
				var env = context.HostingEnvironment;
				config.AddJsonFile("appsettings.json", false, true)
				.AddJsonFile($"appsetting.json.{env.EnvironmentName}.json", true, true)
				.AddEnvironmentVariables();
			});
		}
	}
}
