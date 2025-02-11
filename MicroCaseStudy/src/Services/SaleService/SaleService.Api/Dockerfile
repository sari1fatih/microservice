# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/SaleService/SaleService.Api/SaleService.Api.csproj", "src/Services/SaleService/SaleService.Api/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.Factory/EventBus.Factory.csproj", "src/BuildingBlocks/EventBus/EventBus.Factory/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.AzureServiceBus/EventBus.AzureServiceBus.csproj", "src/BuildingBlocks/EventBus/EventBus.AzureServiceBus/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.Base/EventBus.Base.csproj", "src/BuildingBlocks/EventBus/EventBus.Base/"]
COPY ["src/BuildingBlocks/EventBus/EventBus.RabbitMQ/EventBus.RabbitMQ.csproj", "src/BuildingBlocks/EventBus/EventBus.RabbitMQ/"]
COPY ["src/Services/SaleService/SaleService.Bootstrapper/SaleService.Bootstrapper.csproj", "src/Services/SaleService/SaleService.Bootstrapper/"]
COPY ["src/Services/SaleService/SaleService.Api.ServiceRegistration/SaleService.Api.ServiceRegistration.csproj", "src/Services/SaleService/SaleService.Api.ServiceRegistration/"]
COPY ["src/Cores/Core.Redis/Core.Redis.csproj", "src/Cores/Core.Redis/"]
COPY ["src/Cores/Core.Api/Core.Api.csproj", "src/Cores/Core.Api/"]
COPY ["src/Cores/Core.Security/Core.Security.csproj", "src/Cores/Core.Security/"]
COPY ["src/Cores/Core.WebAPI.Appsettings/Core.WebAPI.Appsettings.csproj", "src/Cores/Core.WebAPI.Appsettings/"]
COPY ["src/Cores/Core.CrossCuttingConcerns/Core.CrossCuttingConcerns.csproj", "src/Cores/Core.CrossCuttingConcerns/"]
COPY ["src/Cores/Core.ElasticSearch/Core.ElasticSearch.csproj", "src/Cores/Core.ElasticSearch/"]
COPY ["src/Services/SaleService/SaleService.Application/SaleService.Application.csproj", "src/Services/SaleService/SaleService.Application/"]
COPY ["src/Cores/Core.Application/Core.Application.csproj", "src/Cores/Core.Application/"]
COPY ["src/Cores/Core.Persistance/Core.Persistance.csproj", "src/Cores/Core.Persistance/"]
COPY ["src/Cores/Core.Mailing.MailKit/Core.Mailing.MailKit.csproj", "src/Cores/Core.Mailing.MailKit/"]
COPY ["src/Cores/Core.Mailing/Core.Mailing.csproj", "src/Cores/Core.Mailing/"]
COPY ["src/Services/SaleService/SaleService.Persistance.Abstract/SaleService.Persistance.Abstract.csproj", "src/Services/SaleService/SaleService.Persistance.Abstract/"]
COPY ["src/Services/SaleService/SaleService.Domain/SaleService.Domain.csproj", "src/Services/SaleService/SaleService.Domain/"]
COPY ["src/Services/SaleService/SaleService.Persistance/SaleService.Persistance.csproj", "src/Services/SaleService/SaleService.Persistance/"]
RUN dotnet restore "./src/Services/SaleService/SaleService.Api/SaleService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/SaleService/SaleService.Api"
RUN dotnet build "./SaleService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SaleService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SaleService.Api.dll"]