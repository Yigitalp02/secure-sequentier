# Backend API Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 9999

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["SecureSolution2/SecureSolution2.csproj", "SecureSolution2/"]
COPY ["Subsystems/SignerApp/SignerApp.csproj", "Subsystems/SignerApp/"]
COPY ["Subsystems/AdvancedSignerApp/AdvancedSignerApp.csproj", "Subsystems/AdvancedSignerApp/"]
RUN dotnet restore "SecureSolution2/SecureSolution2.csproj"
RUN dotnet restore "Subsystems/SignerApp/SignerApp.csproj"
RUN dotnet restore "Subsystems/AdvancedSignerApp/AdvancedSignerApp.csproj"

# Copy everything else and build
COPY SecureSolution2/ ./SecureSolution2/
COPY Subsystems/ ./Subsystems/
WORKDIR "/src/SecureSolution2"
RUN dotnet build "SecureSolution2.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Build subsystem executables
WORKDIR /src/Subsystems/SignerApp
RUN dotnet publish "SignerApp.csproj" -c Release -o /app/subsystems/SignerApp
WORKDIR /src/Subsystems/AdvancedSignerApp
RUN dotnet publish "AdvancedSignerApp.csproj" -c Release -o /app/subsystems/AdvancedSignerApp

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/SecureSolution2"
RUN dotnet publish "SecureSolution2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Copy built subsystem executables
COPY --from=build /app/subsystems /app/subsystems

# Create directories for data persistence
RUN mkdir -p /app/data/watch /app/data/queue /app/data/processed /app/data/logs

ENV ASPNETCORE_URLS=http://+:9999
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SecureSolution2.dll"]
