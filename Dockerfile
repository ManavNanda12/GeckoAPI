# ========================================
# Stage 1: Restore & Build
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install git for submodule support
RUN apt-get update && apt-get install -y git && rm -rf /var/lib/apt/lists/*

WORKDIR /src

# Copy csproj first for better layer caching
COPY *.csproj ./
RUN dotnet restore

# Copy everything else
COPY . .

# Initialize git submodules if they exist
RUN if [ -d .git ] && [ -f .gitmodules ]; then \
      git submodule update --init --recursive; \
    fi

# Build
RUN dotnet build -c Release -o /app/build

# ========================================
# Stage 2: Publish
# ========================================
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ========================================
# Stage 3: Runtime
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
