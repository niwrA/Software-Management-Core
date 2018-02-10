using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using ProductsShared;
using niwrA.CommandManager;
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
using FilesShared;
using Microsoft.AspNetCore.Server.IISIntegration;
using ProductInstallationsShared;
using Microsoft.AspNetCore.Http;

namespace SoftwareManagementCoreApi
{
  public class SpaSettings
  {

  }
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
      services.Configure<IISOptions>(options => {
        options.AutomaticAuthentication = true;
      });

      services.AddCors(options =>
      {
        options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
      });
      services.AddMvc();
      #region "EntityFramework Configuration with SQL Server"
      var connection = $"{Configuration["ConnectionStrings:EntityFramework"]}";
      services.AddDbContext<SoftwareManagementEFCoreRepository.MainContext>(options => options.UseSqlServer(connection));
      #endregion
      SetupDI(services);

      // todo: setup so that both SQL server and MongoDb can be used at the same time and which one to use can be configured
    }
    private static void SetupSQLServerDbDI(IServiceCollection services)
    {
      services.AddTransient<IProductStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<IDesignStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<IProjectStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<IContactStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<IContactStateReadOnlyRepository, SoftwareManagementEFCoreRepository.MainReadOnlyRepository>();
      services.AddTransient<ICompanyStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<ICompanyStateReadOnlyRepository, SoftwareManagementEFCoreRepository.MainReadOnlyRepository>();
      services.AddTransient<ILinkStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<IFileStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<IEmploymentStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<IProjectRoleAssignmentStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<ICommandStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
      services.AddTransient<IProductInstallationStateRepository, SoftwareManagementEFCoreRepository.MainRepository>();
    }
    private static void SetupMongoDbDI(IServiceCollection services)
    {
      services.AddTransient<IMongoClient, MongoClient>();
      // this needs to be here for now, because MainRepository is instanced per Controller
      // see: http://mongodb.github.io/mongo-csharp-driver/2.2/reference/bson/mapping/
      // todo: add tests that expose if these are missing
      BsonClassMap.RegisterClassMap<CompanyRoleState>();
      BsonClassMap.RegisterClassMap<ProjectRoleState>();
      BsonClassMap.RegisterClassMap<ProductVersionState>();
      BsonClassMap.RegisterClassMap<ProductFeatureState>();
      BsonClassMap.RegisterClassMap<ProductIssueState>();
      BsonClassMap.RegisterClassMap<CompanyEnvironmentState>();
      BsonClassMap.RegisterClassMap<CompanyEnvironmentHardwareState>();
      BsonClassMap.RegisterClassMap<EpicElementState>();
      BsonClassMap.RegisterClassMap<EntityElementState>();
      BsonClassMap.RegisterClassMap<PropertyElementState>();
      BsonClassMap.RegisterClassMap<CommandElementState>();
      services.AddTransient<IProductStateRepository, ProductStateRepository>();
      services.AddTransient<IDesignStateRepository, DesignStateRepository>();
      services.AddTransient<IProjectStateRepository, ProjectStateRepository>();
      services.AddTransient<IContactStateRepository, ContactStateRepository>();
      services.AddTransient<IContactStateReadOnlyRepository, ContactStateRepository>();
      services.AddTransient<ICompanyStateRepository, CompanyStateRepository>();
      services.AddTransient<ICompanyStateReadOnlyRepository, CompanyStateRepository>();
      services.AddTransient<ILinkStateRepository, LinkStateRepository>();
      services.AddTransient<IFileStateRepository, FileStateRepository>();
      services.AddTransient<IEmploymentStateRepository, EmploymentStateRepository>();
      services.AddTransient<IProjectRoleAssignmentStateRepository, ProjectRoleAssignmentStateRepository>();
      services.AddTransient<ICommandStateRepository, CommandStateRepository>();
      // todo: productinstallations
    }
    private static void SetupDI(IServiceCollection services)
    {
      //SetupMongoDbDI(services); // uncommnent to use MongoDb
      SetupSQLServerDbDI(services); // comment to use MongoDb
      // helpers
      services.AddTransient<DateTimeShared.IDateTimeProvider, DateTimeProvider>();
      services.AddTransient<niwrA.CommandManager.IDateTimeProvider, DateTimeProvider>();

      // entity services
      services.AddTransient<IProductService, ProductService>();
      services.AddTransient<IDesignService, DesignService>();
      services.AddTransient<IProjectService, ProjectService>();
      services.AddTransient<IContactService, ContactService>();
      services.AddTransient<ICompanyService, CompanyService>();
      services.AddTransient<ILinkDetailsProcessor, LinkDetailsProcessor>();
      services.AddTransient<ILinkService, LinkService>();
      services.AddTransient<IFileService, FileService>();

      // entity relation services
      services.AddTransient<IEmploymentService, EmploymentService>();
      services.AddTransient<IProductInstallationService, ProductInstallationService>();
      services.AddTransient<IProjectRoleAssignmentService, ProjectRoleAssignmentService>();

      // helpers
      services.AddTransient<ICodeGenService, CSharpUpdater>();
      services.AddTransient<ICommandService, CommandService>();
      services.AddTransient<ICommandManager, CommandManager>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseCors("SiteCorsPolicy");
      app.UseStaticFiles();
      app.UseMvc();
      var defaultPage = $"{Configuration["SpaSettings:DefaultPage"]}";

      ConfigureRoutes(app, defaultPage);
    }

    private void ConfigureRoutes(IApplicationBuilder app, string defaultPage)
    {
      // If the route contains '.' then assume a file to be served
      // and try to serve using StaticFiles
      // if the route is spa route then let it fall through to the
      // spa index file and have it resolved by the spa application
      app.MapWhen(context => {
        var path = context.Request.Path.Value;
        return !path.Contains(".");
      },
      spa => {
        spa.Use((context, next) =>
        {
          context.Request.Path = new PathString("/" + defaultPage);
          return next();
        });

        spa.UseStaticFiles();
      });

    }
  }
}
