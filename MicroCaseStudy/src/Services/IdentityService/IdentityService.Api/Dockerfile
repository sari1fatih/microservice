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
COPY ["src/Services/IdentityService/IdentityService.Api/IdentityService.Api.csproj", "src/Services/IdentityService/IdentityService.Api/"]
COPY ["src/Cores/Core.Api/Core.Api.csproj", "src/Cores/Core.Api/"]
COPY ["src/Cores/Core.Security/Core.Security.csproj", "src/Cores/Core.Security/"]
COPY ["src/Cores/Core.WebAPI.Appsettings/Core.WebAPI.Appsettings.csproj", "src/Cores/Core.WebAPI.Appsettings/"]
COPY ["src/Services/IdentityService/IdentityService.Bootstrapper/IdentityService.Bootstrapper.csproj", "src/Services/IdentityService/IdentityService.Bootstrapper/"]
COPY ["src/Services/IdentityService/IdentityService.Api.ServiceRegistration/IdentityService.Api.ServiceRegistration.csproj", "src/Services/IdentityService/IdentityService.Api.ServiceRegistration/"]
COPY ["src/Cores/Core.CrossCuttingConcerns/Core.CrossCuttingConcerns.csproj", "src/Cores/Core.CrossCuttingConcerns/"]
COPY ["src/Cores/Core.ElasticSearch/Core.ElasticSearch.csproj", "src/Cores/Core.ElasticSearch/"]
COPY ["src/Cores/Core.Hangfire/Core.Hangfire.csproj", "src/Cores/Core.Hangfire/"]
COPY ["src/Services/IdentityService/IdentityService.Application/IdentityService.Application.csproj", "src/Services/IdentityService/IdentityService.Application/"]
COPY ["src/Cores/Core.Redis/Core.Redis.csproj", "src/Cores/Core.Redis/"]
COPY ["src/Cores/Core.Application/Core.Application.csproj", "src/Cores/Core.Application/"]
COPY ["src/Cores/Core.Persistance/Core.Persistance.csproj", "src/Cores/Core.Persistance/"]
COPY ["src/Cores/Core.Mailing.MailKit/Core.Mailing.MailKit.csproj", "src/Cores/Core.Mailing.MailKit/"]
COPY ["src/Cores/Core.Mailing/Core.Mailing.csproj", "src/Cores/Core.Mailing/"]
COPY ["src/Services/IdentityService/IdentityService.Domain/IdentityService.Domain.csproj", "src/Services/IdentityService/IdentityService.Domain/"]
COPY ["src/Services/IdentityService/IdentityService.Persistance.Abstract/IdentityService.Persistance.Abstract.csproj", "src/Services/IdentityService/IdentityService.Persistance.Abstract/"]
COPY ["src/Services/IdentityService/IdentityService.Persistance/IdentityService.Persistance.csproj", "src/Services/IdentityService/IdentityService.Persistance/"]
RUN dotnet restore "./src/Services/IdentityService/IdentityService.Api/IdentityService.Api.csproj"
COPY . .
WORKDIR "/src/src/Services/IdentityService/IdentityService.Api"
RUN dotnet build "./IdentityService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./IdentityService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityService.Api.dll"]