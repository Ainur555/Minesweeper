using Minesweeper.BusinessLogic.Services;
using Minesweeper.BusinessLogic.Services.Implementations;
using Minesweeper.Settings;

namespace Minesweeper
{
    public static class Registrar
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var applicationSettings = configuration.Get<ApplicationSettings>();
            services.AddSingleton(applicationSettings)
                    .AddSingleton((IConfigurationRoot)configuration)
                    .InstallServices();
                  //  .InstallRepositories();
            return services;
        }

        private static IServiceCollection InstallServices(this IServiceCollection serviceCollection)
        {
            serviceCollection
                 .AddScoped<IMinesweeperService, MinesweeperService>();            
            return serviceCollection;
        }

        private static IServiceCollection InstallRepositories(this IServiceCollection serviceCollection)
        {
              //To do добавить регистрацию репозиториев

            return serviceCollection;
        }
    }
}
