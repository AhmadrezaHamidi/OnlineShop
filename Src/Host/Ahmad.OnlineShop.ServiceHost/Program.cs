using AhmadBase.Config.Applications;
using Ahmad.OnlineShop.Persistence.EF.Dev;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    var connStr = builder.Configuration.GetConnectionString("CommandConnection") ?? string.Empty;
    await DevSeedData.SeedAsync(connStr);
}

await new WebBuilder(builder)
    .PrintApplicationName()
    .ConfigureEnvironmentVariable()
    .ConfigureSerilog()
    .ConfigureAutofac()
    .ConfigureRedisCache()
    .ConfigureAuthentication()
    .ConfigureController()
    .ConfigureSwagger()
    .ConfigureEndPoints()
    .ConfigureApiVersion()
    .ConfigureCors()
    .RunAsync();

//app.MapAccountEndpoints();

//await WebBuilder.Create(args)
//    .PrintApplicationName()
//    .ConfigureSerilog()
//    .ConfigureAutofac()
//    .ConfigureController()
//    .ConfigureSwagger()
//    .ConfigureAuthentication()
//    .ConfigureApiVersion()
//    .ConfigureCors()
//    .ConfigureAPM()
//    .ConfigurePrometheus()
//    .Build()
//    .DefaultMiddleware()
//    .RequestLoggingMiddleware()
//    .PrometheusMiddleware()
//    .RunAsync();