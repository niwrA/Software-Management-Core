using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;

namespace SoftwareManagementCoreApi
{
    public class Program
    {

        static public IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .ConfigureAppConfiguration((hostContext, config) =>
                {
            // delete all default configuration providers
            config.Sources.Clear();
                    config.AddJsonFile("myconfig.json", optional: true);
                })
                .Build();
    }
}
