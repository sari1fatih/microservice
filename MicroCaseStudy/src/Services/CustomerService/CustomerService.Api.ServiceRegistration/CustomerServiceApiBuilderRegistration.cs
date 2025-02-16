using Core.Api.Middlewares;
using Core.CrossCuttingConcerns.Exceptions.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace CustomerService.Api.ServiceRegistration;

public static class CustomerServiceApiBuilderRegistration
{
    public static void AddCustomerServiceApiBuilderRegistration(this IApplicationBuilder app,
        IWebHostEnvironment environment, IConfiguration configuration)
    {
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.UseCors();
        app.UseRateLimiter();

        app.UseHttpLogging();

        app.UseAuthentication();

        app.UseMiddleware<SessionMiddleware>();

        //if (environment.IsProduction())
        app.ConfigureCustomExceptionMiddleware();

        app.UseDefaultFiles();

        app.UseHttpsRedirection();

        app.UseAuthorization();
    }
}