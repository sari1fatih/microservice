using Core.ElasticSearch.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.ElasticSearch;

public static class CoreElasticSearchRegistration
{
    public static IServiceCollection AddCoreElasticSearchRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ElasticSearchConfig>(configuration.GetSection(nameof(ElasticSearchConfig)));
        services.AddScoped<IElasticSearch, ElasticSearchManager>();
        return services;
    }
}