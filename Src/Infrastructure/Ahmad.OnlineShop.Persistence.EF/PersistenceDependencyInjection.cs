using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ahmad.OnlineShop.Domain.User;

namespace Ahmad.OnlineShop.Persistence.EF;


public static class PersistenceDependencyInjection
{
    //public static IServiceCollection AddPersistenceDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services.AddDbContext<ApplicationDbContext>(options =>
    //            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


    //    services
    //        .AddIdentityCore<User>()
    //        .AddEntityFrameworkStores<ApplicationDbContext>();


    //    return services;
    //}

}