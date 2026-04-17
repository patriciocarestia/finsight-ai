using FinsightAI.Application.Interfaces;
using FinsightAI.Infrastructure.AI;
using FinsightAI.Infrastructure.Data;
using FinsightAI.Infrastructure.ExternalApis;
using FinsightAI.Infrastructure.Repositories;
using FinsightAI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinsightAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IPositionRepository, PositionRepository>();
        services.AddScoped<IRateRepository, RateRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<RatesFetcherService>();
        services.AddScoped<HistoricalDataSeeder>();

        services.AddHttpClient<DolarApiClient>();
        services.AddHttpClient<CoinGeckoClient>(client =>
            client.DefaultRequestHeaders.Add("User-Agent", "FinsightAI/1.0 (portfolio tracker)"));
        services.AddHttpClient<IGeminiClient, GeminiClient>();

        return services;
    }
}
