# Use the ASP.NET runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy all project files
COPY ["GeckoAPI/GeckoAPI.csproj", "GeckoAPI/"]
COPY ["GeckoAPI.Common/GeckoAPI.Common.csproj", "GeckoAPI.Common/"]
COPY ["GeckoAPI.Model/GeckoAPI.Model.csproj", "GeckoAPI.Model/"]
COPY ["GeckoAPI.Repository/GeckoAPI.Repository.csproj", "GeckoAPI.Repository/"]
COPY ["GeckoAPI.Service/GeckoAPI.Service.csproj", "GeckoAPI.Service/"]

# Restore dependencies
RUN dotnet restore "GeckoAPI/GeckoAPI.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/GeckoAPI"
RUN dotnet build "GeckoAPI.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "GeckoAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
