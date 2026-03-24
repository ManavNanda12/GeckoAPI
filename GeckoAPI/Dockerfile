# ========================================
# Stage 1: Build
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy everything
COPY . .

# Restore
RUN dotnet restore

# Build
RUN dotnet build -c Release

# ========================================
# Stage 2: Publish
# ========================================
FROM build AS publish

RUN dotnet publish GeckoAPI/GeckoAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========================================
# Stage 3: Runtime
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=publish /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
