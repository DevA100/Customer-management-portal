using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                //Initialize Logger    
                Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
                try
                {
                    Log.Information("Application Starting.CBN Quarterly Report");
                    CreateHostBuilder(args).Build().Run();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "The Application failed to start.######333");
                }
                finally
                {
                    Log.CloseAndFlush();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
            //CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
           
            Host.CreateDefaultBuilder(args).UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
