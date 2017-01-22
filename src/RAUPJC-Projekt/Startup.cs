using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RAUPJC_Projekt.Core;
using RAUPJC_Projekt.Core.ServiceLogic;
using RAUPJC_Projekt.Core.TermDateLogic;
using RAUPJC_Projekt.Data;
using RAUPJC_Projekt.Models;
using RAUPJC_Projekt.Services;
using Serilog;
using Serilog.Events;

namespace RAUPJC_Projekt
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            // Configure a logger that will log to db
            Log.Logger = new LoggerConfiguration()
                              .Enrich.FromLogContext()
                              .WriteTo.MSSqlServer(Configuration["ConnectionStrings:DefaultConnection"], "Logs", LogEventLevel.Error, autoCreateSqlTable: true)
                              .CreateLogger();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            services.AddMvc(options =>
            {
                options.SslPort = 44333;
                options.Filters.Add(new RequireHttpsAttribute());
            });

            // Create authorization policy for employee and administrator roles
            services.AddAuthorization(options =>
                options.AddPolicy("ElevatedRights", policy =>
                        policy.RequireRole(Constants.AdministratorRoleName, Constants.EmployeeRoleName)));

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<ITermDateRepository, TermDateSqlRepository>();
            services.AddTransient<IServiceRepository, ServiceSqlRepository>();
            services.AddScoped<MyDbContext>(
                s => new MyDbContext(Configuration.GetConnectionString("DefaultConnection")));
           
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(60);
                options.CookieHttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseFacebookAuthentication(new FacebookOptions()
            {
                AppId = Configuration["Authentication:Facebook:AppId"],
                AppSecret = Configuration["Authentication:Facebook:AppSecret"]
            });


            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
           await CreateAdminAndRoles(serviceProvider);
        }

        private async Task CreateAdminAndRoles(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            string[] roleNames =
            {
                Constants.AdministratorRoleName, Constants.EmployeeRoleName,
                Constants.CustomerRoleName
            };

            foreach (var role in roleNames)
            {
                if (!(await roleManager.RoleExistsAsync(role)))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var user = await userManager.FindByEmailAsync("admin@gmail.com");
            if (user != null)
            {
                if (!(await userManager.IsInRoleAsync(user, Constants.AdministratorRoleName)))
                {
                    await userManager.AddToRoleAsync(user, Constants.AdministratorRoleName);
                    await userManager.AddToRoleAsync(user, Constants.EmployeeRoleName);
                    await userManager.AddToRoleAsync(user, Constants.CustomerRoleName);
                }
            }
            else
            {
                user = new ApplicationUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com", 
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, "Ovojesifrazaadmina1!");
                await userManager.AddToRoleAsync(user, Constants.AdministratorRoleName);
                await userManager.AddToRoleAsync(user, Constants.EmployeeRoleName);
                await userManager.AddToRoleAsync(user, Constants.CustomerRoleName);
            }
        }


    }
}
