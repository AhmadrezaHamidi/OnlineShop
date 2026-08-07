using Base.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace Base.WebApi
{
    public static class ServiceCollectionExtenshions
    {
        public static IServiceCollection AddSwaggerServies(this IServiceCollection services)
        {
           services.AddSwaggerGen(c =>
            {
                var securityScheme = new OpenApiSecurityScheme
                {
                    Description = "Bearer {token}",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Base.Api",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", securityScheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new [] {"Bearer"} }
                });
            });


            return services;
        }
    }
}
