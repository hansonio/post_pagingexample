using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PagingExample.Models;

namespace PagingExample.Data.SeedData
{
    public static class SeedDataServiceExtensions
    {
        public static async Task EnsureSeedData(this IHostBuilder builder, string[] args)
        {
            builder
            .ConfigureLogging(x => {
                x.AddConsole();
            });

            var host = builder.Build();

            var services =  host.Services.CreateScope();
            var service = services.ServiceProvider.GetService<SeedDataService>();

            await service.EnsureSeedData();
        }
    }

    public class SeedDataService
        {
        private readonly ILogger<SeedDataService> _logger;
        private readonly ApplicationDbContext _dataContext;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public SeedDataService(ILogger<SeedDataService> logger
                                , ApplicationDbContext dataContext
                                , RoleManager<ApplicationRole> roleManager
                                , UserManager<ApplicationUser> userManager
                                )
        {
            _logger = logger;
            _dataContext = dataContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task EnsureSeedData()
        {
            _logger.LogInformation("Starting to Ensure Seed Data");

            await MigrateSchema();
            await EnsureRoles();
            await EnsureUsers();
        }

        protected async Task MigrateSchema(){
            _logger.LogInformation("Migrating Database Schema");
            await _dataContext.Database.MigrateAsync();
        }

        protected async Task EnsureRoles(){
            _logger.LogInformation("Ensure Seed Roles");

            var seedRoles = new []{"admin", "orgadmin"};

            var roles = await _dataContext.Roles.Select(x => x.Name.ToLower()).ToListAsync();

            foreach(var r in seedRoles.Where(x => !roles.Contains(x))){
                _logger.LogInformation($"Creating role '{r}'");
                await _roleManager.CreateAsync(new ApplicationRole(){Id = Guid.NewGuid().ToString(), Name = r});
            }
        }

        protected async Task EnsureUsers(){
            _logger.LogInformation("Ensure Seed Users");

            var seedUsers = new []{
                new {Id = "f2cdb728-b176-46d3-9546-131328a061be", Email = "admin@inertiafx.local", Password = "P@ssword2", Roles = new[]{"admin", "orgadmin"} }
                //Add additional users here
            };

            foreach(var u in seedUsers){
                var user = await _userManager.FindByEmailAsync(u.Email);
                if(user == null){
                    _logger.LogInformation($"Creating user '{u.Email}'");

                    user = new ApplicationUser(){
                        Id = u.Id,
                        Email = u.Email,
                        EmailConfirmed = true,
                        UserName = u.Email
                    };
                    await _userManager.CreateAsync(user, u.Password);
                    await _userManager.AddToRolesAsync(user, u.Roles);
                }
            }
        }
    }
}