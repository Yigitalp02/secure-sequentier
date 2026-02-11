# Frontend Web Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5188

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Web/Web.csproj", "Web/"]
COPY ["SecureSolution2/SecureSolution2.csproj", "SecureSolution2/"]
RUN dotnet restore "Web/Web.csproj"

# Copy everything else and build
COPY Web/ ./Web/
COPY SecureSolution2/ ./SecureSolution2/
WORKDIR "/src/Web"
RUN dotnet build "Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5188
ENV ASPNETCORE_ENVIRONMENT=Production
ENV BACKEND_API_URL=http://backend:9999

ENTRYPOINT ["dotnet", "Web.dll"]

