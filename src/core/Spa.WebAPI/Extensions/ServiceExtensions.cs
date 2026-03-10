namespace Spa.WebAPI.Extensions;

public static class ServiceExtensions
{
    internal static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        //var emailSettings = configuration.GetSection(key: nameof(SMTPEmailSetting))
        //	.Get<SMTPEmailSetting>();
        //services.AddSingleton(emailSettings);

        //var eventBusSettings = configuration.GetSection(key: nameof(EventBusSettings))
        //	.Get<EventBusSettings>();
        //services.AddSingleton(eventBusSettings);

        return services;
    }
}