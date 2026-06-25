using AhmadBase.Config.Applications;

await new WebBuilder(WebApplication.CreateBuilder(args))
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