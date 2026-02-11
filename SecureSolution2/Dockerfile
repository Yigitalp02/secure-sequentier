# Backend API Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 9999

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["SecureSolution2/SecureSolution2.csproj", "SecureSolution2/"]
RUN dotnet restore "SecureSolution2/SecureSolution2.csproj"

# Copy everything else and build
COPY SecureSolution2/ ./SecureSolution2/
WORKDIR "/src/SecureSolution2"
RUN dotnet build "SecureSolution2.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SecureSolution2.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Create directories for data persistence
RUN mkdir -p /app/data/watch /app/data/queue /app/data/processed /app/data/logs

ENV ASPNETCORE_URLS=http://+:9999
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SecureSolution2.dll"]

