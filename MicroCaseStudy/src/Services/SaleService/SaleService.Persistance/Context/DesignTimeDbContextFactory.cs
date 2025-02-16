using Core.CrossCuttingConcerns.Serilog;
using Core.CrossCuttingConcerns.Serilog.Loggers;
using Core.WebAPI.Appsettings;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace SaleService.Persistance.Context;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SaleServiceDbContext>
{
    #region Implementation of IDesignTimeDbContextFactory<out BaseDbContext>

    public SaleServiceDbContext CreateDbContext(string[] args)
    {
        // Mock bağımlılıkları veya temel bir yapılandırma
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Development.json") // Varsayılan ayarlar
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SaleServiceDbContext>();
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("SaleService"));

        var httpContextAccessor = new HttpContextAccessor(); // Basit bir örnek, boş bir HttpContext döner.

        var userSession = new UserSession<int>()
        {
            UserId = 1,
            Email = "fatihsari1992@gmail.com",
            Username = "sari1fatih",
            Name = "Fatih",
            Surname = "Sarı",
            Jti = "f4f8eb78-606e-4a18-bbfd-147727f5ffa6",
            Body = string.Empty
        }; // IUserSession<int> için basit bir mock
        LoggerServiceBase loggerServiceBase = new PostgreSqlLogger(configuration);
        
        var webApiConfiguration = Options.Create(configuration.GetSection(nameof(WebApiConfiguration)).Get<WebApiConfiguration>()??new WebApiConfiguration()); //
        
        return new SaleServiceDbContext(
            optionsBuilder.Options,
            configuration,
            httpContextAccessor,
            loggerServiceBase,
            userSession,
            webApiConfiguration
        );
    }

    #endregion
}