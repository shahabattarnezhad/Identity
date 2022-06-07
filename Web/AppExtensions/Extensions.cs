using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Web.Data;
using Web.Services.EmailService;

namespace Web.AppExtensions
{
    public static class Extensions
    {
        public static void ConfigureSqlServerConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders()
                    .AddDefaultUI();
        }

        public static void ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
            });
        }

        public static void ConfigureEmailSetting(this IServiceCollection services)
        {
            services.AddScoped<IEmailSender, EmailService>();
        }

        public static void ConfigureAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserAndAdmin", policy =>
                {
                    policy.RequireRole("Admin")
                          .RequireRole("User");
                });
                options.AddPolicy("Admin_CreateAccess", policy =>
                {
                    policy.RequireRole("Admin")
                          .RequireClaim("Create", "True");
                });
                options.AddPolicy("Admin_Create_Edit_DeleteAccess", policy =>
                {
                    policy.RequireRole("Admin")
                          .RequireClaim("Create", "True")
                          .RequireClaim("Edit", "True")
                          .RequireClaim("Delete", "True");
                });
                options.AddPolicy("Admin_Create_Edit_DeleteAccess_Or_SuperAdmin", policy =>
                {
                    policy.RequireAssertion(context =>
                    (
                        context.User.IsInRole("Admin") &&
                        context.User.HasClaim(c => c.Type == "Create" && c.Value == "True") &&
                        context.User.HasClaim(c => c.Type == "Edit" && c.Value == "True") &&
                        context.User.HasClaim(c => c.Type == "Delete" && c.Value == "True")
                    ) || context.User.IsInRole("SuperAdmin"));
                });
            });
        }
    }
}
