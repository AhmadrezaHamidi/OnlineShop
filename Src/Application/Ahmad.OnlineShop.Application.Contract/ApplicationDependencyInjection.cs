using Ahmad.OnlineShop.Application.Contract.Opetions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ahmad.OnlineShop.Application.Contract;


public static class ApplicationDependencyInjection
{
    public static IServiceCollection AddPersistenceDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {

        return services;
    }
}
