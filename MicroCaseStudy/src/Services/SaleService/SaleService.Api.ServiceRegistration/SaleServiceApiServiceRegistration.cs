using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.RateLimiting;
using Core.Security.JWT;
using Core.WebAPI.Appsettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SaleService.Api.ServiceRegistration.Handlers;

namespace SaleService.Api.ServiceRegistration;

public static class SaleServiceApiServiceRegistration
{
     public static void AddSaleServiceApiServiceRegistration(this IServiceCollection services,
        IConfiguration configuration)
    {
         OptionsConfigurationServiceCollectionExtensions.Configure<WebApiConfiguration>(services, configuration.GetSection(nameof(WebApiConfiguration)));

        var webApiConfiguration = configuration.GetSection(nameof(WebApiConfiguration)).Get<WebApiConfiguration>() ??
                                  throw new InvalidOperationException(
                                      $"\"{nameof(WebApiConfiguration)}\" section cannot found in configuration.");

        OptionsConfigurationServiceCollectionExtensions.Configure<RedisConfigurations>(services, configuration.GetSection(nameof(RedisConfigurations)));

        OptionsConfigurationServiceCollectionExtensions.Configure<GeneralSettings>(services, configuration.GetSection(nameof(GeneralSettings)));

        var redisConfigurations = configuration.GetSection(nameof(RedisConfigurations)).Get<RedisConfigurations>();
        var ratelimitingSettings = configuration.GetSection(nameof(RatelimitingSettings)).Get<RatelimitingSettings>();


        OptionsConfigurationServiceCollectionExtensions.Configure<TokenOptions>(services, configuration.GetSection(nameof(TokenOptions)));

        TokenOptions tokenOptions = configuration.GetSection(nameof(TokenOptions)).Get<TokenOptions>();

        StackExchangeRedisCacheServiceCollectionExtensions.AddStackExchangeRedisCache(services, opt =>
            opt.Configuration = redisConfigurations?.ConnectionString ?? string.Empty);
        
        HttpServiceCollectionExtensions.AddHttpContextAccessor(services);
        
         services.AddRateLimiter(options =>
        {
            
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.ContentType = "application/json"; // JSON formatında response
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                var response = new
                {
                    ApiResultType = 2,
                    Data = (object)null,
                    Message = new[] { "Too many requests. Please slow down and try again later." }
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                await context.HttpContext.Response.WriteAsync(jsonResponse, cancellationToken);
            }; 
            
            options.AddPolicy("RateLimitUserId", context =>
                RateLimitPartition.GetFixedWindowLimiter(partitionKey: context.User?.Claims?.FirstOrDefault(x => x
                    .Type == CustomClaimKeys.Id)?.Value,
                    factory: _ => new FixedWindowRateLimiterOptions()
                    {
                        PermitLimit = ratelimitingSettings.PermitLimit,
                        Window = TimeSpan.FromSeconds(ratelimitingSettings.WindowSeconds)
                    }
                ));
            
            options.AddPolicy("RateLimitIp", context =>
                RateLimitPartition.GetFixedWindowLimiter(partitionKey: context.Connection.RemoteIpAddress?.ToString(),
                    factory: _ => new FixedWindowRateLimiterOptions()
                    {
                        PermitLimit = ratelimitingSettings.PermitLimit,
                        Window = TimeSpan.FromSeconds(ratelimitingSettings.WindowSeconds)
                    }
                ));
            
            
        });


        var rsa = RSA.Create();
        rsa.ImportFromPem(tokenOptions.PublicKey);

        var key = new RsaSecurityKey(rsa);
        
        JwtBearerExtensions.AddJwtBearer(AuthenticationServiceCollectionExtensions.AddAuthentication(services, options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }), options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = tokenOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = tokenOptions.Audience,
                    ValidateLifetime = true ,// Token süresinin geçerliliğini kontrol et
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddLogging(configure =>
        { 
            configure.AddConsole();
            configure.AddDebug();
        });
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "SaleService.Api",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
 
        services.AddAuthorization(options =>
        {
            options.AddPolicy("TokenAuthorizationHandler",
                policy => policy.Requirements.Add(new TokenAuthorizationRequirement()));

            options.InvokeHandlersAfterFailure = false;
        });

        services.AddScoped<IAuthorizationHandler, TokenAuthorizationHandler>();
          
        services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestHeaders.Add("sec_ch_ua");
            logging.MediaTypeOptions.AddText("application/javascript");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(webApiConfiguration.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

        services.AddEndpointsApiExplorer();
        
        services.AddControllers();
    }
}