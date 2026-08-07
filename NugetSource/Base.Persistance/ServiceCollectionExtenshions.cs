using Base.Application.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Base.Application.Services.Auth.AuthManeger;
using Base.Domain.Entities.userInf;
using Base.Persistance.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using AutoMapper.Configuration;
using Base.Application.Dtos.ConfigDtos;

namespace Base.Persistance
{
    public static class ServiceCollectionExtenshions
    {
        public static IServiceCollection AddDataServices(this IServiceCollection services,
            Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddDbContext<ApplicationManegerDbConext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("Local"));
            });

            //services.AddAutoMapper(Assembly.GetExecutingAssembly());
            //services.AddEntityFrameworkSqlServer()
            //   .AddDbContext<ApplicationManegerDbConext>();

            services.AddIdentity<User, Role>(option => option.SignIn.RequireConfirmedAccount = false)
        .AddEntityFrameworkStores<ApplicationManegerDbConext>()
        .AddDefaultTokenProviders();


            //services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            //services.AddScoped<ITicketService, TicketService>();
            //services.AddScoped<ICategoryService, CategoryService>();


            services.AddScoped<ITokenInfoService, TokenInfoService>();
            services.AddScoped<IUserService, UserService>();

     
            ///TODO NIMIDonam Kodomo bezaram ya mele khodam ya 



            //services.Add(new TokenInfoService(ApplicationManegerDbConext))

            var jwtSettings = new JwtSettings();
            configuration.Bind(key: nameof(jwtSettings), jwtSettings);
            services.AddSingleton(jwtSettings);

            services.Configure<IdentityOptions>(options =>
            {
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = false;
                options.Lockout.AllowedForNewUsers = false;
                options.SignIn.RequireConfirmedEmail = false;
            });


            services.AddScoped<AppUserManager>();
            services.AddScoped<AppSignInManager>();
            services.AddScoped<AppRoleManager>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                      JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                      JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.RequireHttpsMetadata = false;
               options.SaveToken = true;
               options.ClaimsIssuer = configuration["Authentication:JwtIssuer"];

               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidIssuer = configuration["Authentication:JwtIssuer"],

                   ValidateAudience = true,
                   ValidAudience = configuration["Authentication:JwtAudience"],

                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtKey"])),
                   RequireExpirationTime = true,
                   ValidateLifetime = true,
                   ClockSkew = TimeSpan.Zero
               };
           });


            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromDays(3));

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            return services;


        }
    }
}

