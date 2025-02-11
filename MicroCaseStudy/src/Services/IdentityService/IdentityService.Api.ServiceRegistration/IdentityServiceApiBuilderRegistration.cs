using Core.Api.Middlewares;
using Core.Hangfire.Settings;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IdentityService.Api.ServiceRegistration;

public static class IdentityServiceApiBuilderRegistration
{
    public static void AddIdentityServiceApiBuilderRegistration(this IApplicationBuilder app,
        IWebHostEnvironment environment,IConfiguration configuration)
    {
        var hangfireSettings = configuration.GetSection(nameof(HangfireSettings)).Get<HangfireSettings>();
        if (environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRateLimiter();
        
        app.UseHttpLogging();
        app.UseCors();

        app.UseHangfireDashboard("/jobs", new DashboardOptions
        {
            DashboardTitle = "MicroCaseStudy",
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter{
                    User = hangfireSettings?.UserName,
                    Pass = hangfireSettings?.Password
                }
            }
        });
        
        app.UseAuthentication();
     
        app.UseMiddleware<SessionMiddleware>();
        
        //if (environment.IsProduction())
        //app.ConfigureCustomExceptionMiddleware();
   
        app.UseDefaultFiles(); 
   
        app.UseHttpsRedirection();
           
        app.UseAuthorization();
    }
}