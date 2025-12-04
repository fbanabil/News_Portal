using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News_Portal.Core.DemoData;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.ServiceContracts;
using News_Portal.Core.Services;
using News_Portal.Infrastructure.DbContext;
using News_Portal.Infrastructure.Repositories;
using News_Portal.UI.Samples;
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



            var cloudName = builder.Configuration["Cloudinary:CloudName"];
            var apiKey = builder.Configuration["Cloudinary:ApiKey"];
            var apiSecret = builder.Configuration["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            var cloudinary = new Cloudinary(account);

            builder.Services.AddSingleton(cloudinary);


            services.AddScoped<INewsService, NewsService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<ICommentService, CommentService>();

            //services.AddScoped<NewsDemoData>();
            services.AddScoped<IUpdateSample,UpdateSample>();

            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();


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
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

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
