# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

# Install font-related dependencies for Linux (Telerik Reporting)
RUN apt-get update && apt-get install -y --no-install-recommends \
    libfreetype6 \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy NuGet.Config and local packages
COPY ["NuGet.Config", "."]
COPY ["LocalNugetPackages", "LocalNugetPackages"]

#Copy Reports project files
COPY ["Reports", "Reports"]

COPY ["ReportingEngine.csproj", "."]
RUN dotnet restore "./ReportingEngine.csproj" --configfile NuGet.Config

COPY . .
WORKDIR "/src/."
RUN dotnet build "./ReportingEngine.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ReportingEngine.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportingEngine.dll"]