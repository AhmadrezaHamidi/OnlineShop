using AhmadBase.Config.Containers;
using Autofac;
using Microsoft.Extensions.Configuration;

namespace Ahmad.OnlineShop.Config;

public class AutofacModule : Autofac.Module
{
    private readonly IConfiguration _configuration;

    public AutofacModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        Container.Setup(builder, _configuration)
            .RegisterDatabaseModels()
            .RegisterMainServices()
            .RegisterHandlers()
            .RegisterAuxiliaryServices();
    }
}
