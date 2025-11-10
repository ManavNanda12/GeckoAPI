# Use the ASP.NET runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use SDK for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore and build from solution file if you have one, or the main project
RUN dotnet restore "GeckoAPI.sln"

# Build
RUN dotnet build "GeckoAPI.sln" -c Release

# Publish the main API project
RUN dotnet publish "GeckoAPI/GeckoAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]