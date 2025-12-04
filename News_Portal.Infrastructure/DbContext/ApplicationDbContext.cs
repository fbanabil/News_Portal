using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using News_Portal.Core.Domain.Entities;
using News_Portal.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace News_Portal.Infrastructure.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        //Dbsets

        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Images> Images { get; set; }
        public virtual DbSet<Comments> Comments { get; set; }




        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.HasDefaultSchema("News_Portal");

            modelbuilder.Entity<News>().ToTable("News");
            modelbuilder.Entity<Images>().ToTable("Images");
            modelbuilder.Entity<Comments>().ToTable("Comments");

        }








    }
}
