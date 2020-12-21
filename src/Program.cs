using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PagingExample.Data.SeedData;

namespace PagingExample
{
    public class Program
{
        public static void Main(string[] args)
        {
            var builder = CreateHostBuilder(args);

            if(args.Contains("/seed")){
                builder.EnsureSeedData(args).Wait();
                return;
            }

            builder.Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
