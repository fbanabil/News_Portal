using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Infrastructure.DbContext;
using System.Runtime.CompilerServices;

namespace News_Portal.UI.StartupExtensions
{
    public static class ServiceConfigurationExtensiion
    {
        public static void ConfigureServices(this IServiceCollection services, WebApplicationBuilder? builder)
        {
            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddRouting();



            services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options.UseSqlServer(builder?.Configuration.GetConnectionString("DefaultConnection"));
                }
            );


            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>()
                ;


            //services.AddAuthorization(options =>
            //{
            //    options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
            //        .Build();
            //});

            services.ConfigureApplicationCookie(options =>
            {
                //options.LoginPath="/Account/Login";
            });

        }
    }
}
