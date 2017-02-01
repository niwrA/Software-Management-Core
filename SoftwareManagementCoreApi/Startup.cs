using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProductsShared;
using CommandsShared;
using SoftwareManagementEFCoreRepository;
using Microsoft.EntityFrameworkCore;
using DateTimeShared;
using Microsoft.AspNetCore.Cors.Infrastructure;
using ProjectsShared;
using ContactsShared;
using CompaniesShared;

namespace SoftwareManagementCoreApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            var corsBuilder = new CorsPolicyBuilder();
            corsBuilder.AllowAnyHeader();
            corsBuilder.AllowAnyMethod();
            corsBuilder.AllowAnyOrigin(); // For anyone access.
            //corsBuilder.WithOrigins("http://localhost:56573"); // for a specific url. Don't add a forward slash on the end!
            corsBuilder.AllowCredentials();

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
            });
            services.AddMvc();

            var connection = @"Server=localhost;Database=SoftwareManagement;Trusted_Connection=True;";
            services.AddDbContext<MainContext>(options => options.UseSqlServer(connection));

            // helpers
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();

            // modules (always repo first, then service etc.)
            services.AddTransient<IProductStateRepository, MainRepository>();
            services.AddTransient<IProductService, ProductService>();

            services.AddTransient<IProjectStateRepository, MainRepository>();
            services.AddTransient<IProjectService, ProjectService>();

            services.AddTransient<IContactStateRepository, MainRepository>();
            services.AddTransient<IContactService, ContactService>();

            services.AddTransient<ICompanyStateRepository, MainRepository>();
            services.AddTransient<ICompanyService, CompanyService>();

            services.AddTransient<ICommandStateRepository, MainRepository>();
            services.AddTransient<ICommandManager, CommandManager>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseCors("SiteCorsPolicy");

            app.UseMvc();
        }
    }
}
