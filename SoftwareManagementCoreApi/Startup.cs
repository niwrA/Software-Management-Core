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
using Microsoft.EntityFrameworkCore;
using DateTimeShared;
using Microsoft.AspNetCore.Cors.Infrastructure;
using ProjectsShared;
using ContactsShared;
using CompaniesShared;
using EmploymentsShared;
//using SoftwareManagementEFCoreRepository; // enable to switch to efcorerepo
using SoftwareManagementMongoDbCoreRepository; // enable to switch to mongodb repo
using MongoDB.Driver;
using MongoDB.Bson.Serialization;
using ProjectRoleAssignmentsShared;
using LinksShared;
using DesignsShared;
using CodeGenShared;
using CodeGen;

namespace SoftwareManagementCoreApi
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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

            #region "EntityFramework Configuration with SQL Server"
            //var connection = $"{Configuration["ConnectionStrings:EntityFramework"]}";
            //services.AddDbContext<MainContext>(options => options.UseSqlServer(connection));
            #endregion
            SetupDI(services);
        }

        private static void SetupDI(IServiceCollection services)
        {
            #region "MongoDb Config"
            services.AddTransient<IMongoClient, MongoClient>();
            // this needs to be here for now, because MainRepository is instanced per Controller
            // see: http://mongodb.github.io/mongo-csharp-driver/2.2/reference/bson/mapping/
            // todo: add tests that expose if these are missing
            BsonClassMap.RegisterClassMap<CompanyRoleState>();
            BsonClassMap.RegisterClassMap<ProjectRoleState>();
            BsonClassMap.RegisterClassMap<ProductVersionState>();
            BsonClassMap.RegisterClassMap<CompanyEnvironmentState>();
            BsonClassMap.RegisterClassMap<EpicElementState>();
            BsonClassMap.RegisterClassMap<EntityElementState>();
            BsonClassMap.RegisterClassMap<PropertyElementState>();
            BsonClassMap.RegisterClassMap<CommandElementState>();
            #endregion

            // helpers
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();

            // modules (always repo first, then service etc.)
            // note that I've used the same repo here for all, but that's just one of the options
            services.AddTransient<IProductStateRepository, MainRepository>();
            services.AddTransient<IProductService, ProductService>();

            services.AddTransient<IDesignStateRepository, DesignStateRepository>();
            services.AddTransient<IDesignService, DesignService>();

            services.AddTransient<IProjectStateRepository, MainRepository>();
            services.AddTransient<IProjectService, ProjectService>();

            services.AddTransient<IContactStateRepository, MainRepository>();
            services.AddTransient<IContactService, ContactService>();

            services.AddTransient<ICompanyStateRepository, MainRepository>();
            services.AddTransient<ICompanyService, CompanyService>();

            services.AddTransient<ILinkDetailsProcessor, LinkDetailsProcessor>();
            services.AddTransient<ILinkStateRepository, MainRepository>();
            services.AddTransient<ILinkService, LinkService>();

            services.AddTransient<IEmploymentStateRepository, MainRepository>();
            services.AddTransient<IEmploymentService, EmploymentService>();

            services.AddTransient<IProjectRoleAssignmentStateRepository, MainRepository>();
            services.AddTransient<IProjectRoleAssignmentService, ProjectRoleAssignmentService>();

            services.AddTransient<ICodeGenService, CSharpUpdater>();

            services.AddTransient<ICommandStateRepository, MainRepository>();
            services.AddTransient<ICommandService, CommandService>();
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
