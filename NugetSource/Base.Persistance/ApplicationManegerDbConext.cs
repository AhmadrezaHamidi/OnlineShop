using Base.Domain.Entities;
using Base.Domain.Entities.TallarEntities;
using Base.Domain.Entities.userInf;
using Base.Persistance.EntityConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Base.Persistance
{
    public class ApplicationManegerDbConext : IdentityDbContext<User, Role, int, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {

        public ApplicationManegerDbConext(DbContextOptions<ApplicationManegerDbConext> options)
     : base(options)
        {
        }



        public DbSet<Tallar> Tallar { get; set; }
        public DbSet<OptionsTallar> Optiones { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TallarEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OptionsTallarEntityConfiguration());
            //modelBuilder.ApplyConfiguration(new MissionEntityConfiguration());


            base.OnModelCreating(modelBuilder);
        }
    }

}
