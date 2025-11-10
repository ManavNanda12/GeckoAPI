# Use the ASP.NET runtime as base - UPDATED TO .NET 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

# Use SDK for building - UPDATED TO .NET 9.0
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies for the main project
RUN dotnet restore "GeckoAPI.csproj"

# Build
RUN dotnet build "GeckoAPI.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "GeckoAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]