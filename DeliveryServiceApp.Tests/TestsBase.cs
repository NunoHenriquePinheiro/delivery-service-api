using DeliveryServiceApp.Algorithms.Implementations;
using DeliveryServiceApp.Algorithms.Interfaces;
using DeliveryServiceApp.Cache;
using DeliveryServiceApp.Helpers.Handlers;
using DeliveryServiceApp.Helpers.Settings;
using DeliveryServiceApp.Persistence.Contexts;
using DeliveryServiceApp.Persistence.Repositories.Implementations;
using DeliveryServiceApp.Persistence.Repositories.Interfaces;
using DeliveryServiceApp.Services.Implementations;
using DeliveryServiceApp.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeliveryServiceApp.Tests
{
    /// <summary>Base class for the tests.</summary>
    public class TestsBase
    {
        protected readonly IPointService _pointService;
        protected readonly IRouteBaseService _routeBaseService;
        protected readonly IRouteStepService _routeStepService;
        protected readonly IStepService _stepService;
        protected readonly IUserService _userService;

        public TestsBase()
        {
            // Configure application services logic through DI
            var services = new ServiceCollection();

            ConfigureMemory(services);
            ConfigureAppSettings(services);
            ConfigureInterfaces(services);

            var serviceProvider = services.BuildServiceProvider();
            _pointService = serviceProvider.GetService<IPointService>();
            _routeBaseService = serviceProvider.GetService<IRouteBaseService>();
            _routeStepService = serviceProvider.GetService<IRouteStepService>();
            _stepService = serviceProvider.GetService<IStepService>();
            _userService = serviceProvider.GetService<IUserService>();

            var context = serviceProvider.GetService<DataContext>();
            context.Database.EnsureCreated();
        }


        #region Private methods

        private static void ConfigureMemory(ServiceCollection services)
        {
            services.AddDbContext<DataContext>(x => x.UseInMemoryDatabase("DeliveryServiceDb"));
            services.AddMemoryCache();
        }

        private static void ConfigureAppSettings(ServiceCollection services)
        {
            var configuration = InitConfiguration();
            var appSettingsSection = configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
        }

        private static IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }

        private static void ConfigureInterfaces(ServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICacheManager, CacheManager>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPointRepository, PointRepository>();
            services.AddScoped<IRouteBaseRepository, RouteBaseRepository>();
            services.AddScoped<IStepRepository, StepRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPointService, PointService>();
            services.AddScoped<IRouteBaseService, RouteBaseService>();
            services.AddScoped<IStepService, StepService>();

            services.AddScoped<IComputeRoutePaths, ComputeRoutePaths>();
            services.AddScoped<IRouteStepService, RouteStepService>();

            services.AddTransient<IAuthorizationHandler, RolesInDBAuthorizationHandler>();
        }
        
        #endregion
    }
}
