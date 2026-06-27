using Ahmad.OnlineShop.Persistence.EF.Services.Sms;
using Identity.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ahmad.OnlineShop.Persistence.EF.Services;

public static class SmsServiceExtensions
{
    public static IServiceCollection AddSmsService(
        this IServiceCollection services,
        IConfiguration          configuration)
    {
        services.Configure<SmsOptions>(
            configuration.GetSection(SmsOptions.Section));

        services.AddScoped<ISmsService, SmsSender>();

        return services;
    }
}
