using CustomerService.Api.ServiceRegistration;
using CustomerService.Bootstrapper;

var builder = WebApplication.CreateBuilder(args);

IConfiguration configuration;
ConfigureHostBuilder configureHostBuilder;
IWebHostEnvironment environment;

IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();
// Add services to the container.

environment = builder.Environment;
configureHostBuilder = builder.Host;
configuration = configurationBuilder.Build();


builder.Services.AddCustomerServiceBootstrapperServiceRegistration(configuration);
var app = builder.Build();

app.AddCustomerServiceApiBuilderRegistration(app.Environment, configuration);

app.MapControllers();

app.Run();