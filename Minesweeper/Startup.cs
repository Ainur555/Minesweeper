using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.Mapping;
using Minesweeper.Middlewares;
using Minesweeper.Settings;

namespace Minesweeper
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        const string Origin = "MinesweeperSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var corsConfig = Configuration.GetSection("CorsSettings").Get<CorsSettings>();

            services.AddCors(options =>
            {
                options.AddPolicy(name: Origin,
                                  build =>
                                  {
                                      build.WithOrigins(corsConfig.Origins)
                                      .WithMethods(corsConfig.Methods)
                                      .WithHeaders(corsConfig.Headers);
                                  });
            });

            InstallAutomapper(services);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
            });

            services.AddServices(Configuration);
            services.AddControllers();
            services.AddHealthChecks().AddCheck<MinesweeperHealthCheck>("minesweeperHealth", tags: new string[] { "minesweeperHealthCheck" });
            services.AddFluentValidationAutoValidation();
            services.AddValidators();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray();
                    var result = new { error = string.Join("; ", errors) };
                    return new BadRequestObjectResult(result);
                };
            });

            services.AddOpenApiDocument(options =>
            {
                options.Title = "Minesweeper API doc";
                options.Version = "1.0";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(Origin);

            app.UseOpenApi();
            app.UseSwaggerUi(x =>
            {
                x.DocExpansion = "list";
            });

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseHealthChecks("/minesweeperHealth", new HealthCheckOptions()
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("minesweeperHealthCheck")
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static IServiceCollection InstallAutomapper(IServiceCollection services)
        {
            services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));
            return services;
        }

        private static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<GameMappingsProfiler>();              
            });

            configuration.AssertConfigurationIsValid();
            return configuration;
        }
    }
}
