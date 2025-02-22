using FluentValidation;
using Minesweeper.Valdiators;

namespace Minesweeper.Settings
{
    public static class ServiceCollectionExtensions
    {
        public static void AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<GameTurnRequestValidator>();
            services.AddValidatorsFromAssemblyContaining<NewGameRequestValidator>();
        }
    }
}
