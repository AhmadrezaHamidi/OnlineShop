using AutoMapper.Configuration;
using Base.Application.Contracts;
using Base.Application.Features.command.EmployeeCommands;
using Base.Application.Services.Auth;
using Base.Application.Services.Auth.AuthManeger;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Base.Application
{
    public static class ServiceCollectionExtenshions
    {

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connection)
        {
           
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAutoMapper(Assembly.GetExecutingAssembly());


            return services;
        }

    }
}
