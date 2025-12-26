using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using News_Portal.Core.Domain.IdentityEntities;
using News_Portal.Core.Domain.RepositoryContracts;
using News_Portal.Core.ServiceContracts;
using News_Portal.Core.Services;
using News_Portal.Infrastructure.DbContext;
using News_Portal.Infrastructure.Repositories;
using News_Portal.UI.Samples;
using NuGet.Protocol;
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
                options.Password.RequireDigit = false;
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

            builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            {
                options.ValidationInterval = TimeSpan.Zero;
            });

            builder.Services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                });


            services.AddAuthorization();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath= "/Identity/Account/AuthLogin";

                options.Events.OnRedirectToLogin = context =>
                {
                    var tempFactory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                    var tempData = tempFactory.GetTempData(context.HttpContext);
                    tempData["Error"] = "Please sign in to continue.";
                    tempData.Save();

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };

                options.Events.OnRedirectToAccessDenied = context =>
                {
                    var tempFactory = context.HttpContext.RequestServices.GetRequiredService<ITempDataDictionaryFactory>();
                    var tempData = tempFactory.GetTempData(context.HttpContext);
                    tempData["Error"] = "You do not have permission to access this resource.";
                    tempData.Save();

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
           
            });

        }
    }
}
