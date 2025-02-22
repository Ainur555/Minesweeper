using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Minesweeper.Middlewares
{
    //Тут заготовка под HealthCheck
    public class MinesweeperHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return new HealthCheckResult(HealthStatus.Healthy);
        }
    }
}
