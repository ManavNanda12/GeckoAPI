# Stage 1: Build and Restore
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all csproj files first (for caching and restore)
COPY GeckoAPI.csproj ./
COPY GeckoAPI.Model/GeckoAPI.Model.csproj GeckoAPI.Model/
COPY GeckoAPI.Repository/GeckoAPI.Repository.csproj GeckoAPI.Repository/
COPY GeckoAPI.Service/GeckoAPI.Service.csproj GeckoAPI.Service/

# Restore dependencies
RUN dotnet restore "GeckoAPI.csproj"

# Copy all source code
COPY . .

# Build
RUN dotnet build "GeckoAPI.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "GeckoAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
