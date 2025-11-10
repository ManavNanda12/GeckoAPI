# ========================================
# Stage 1: Restore & Build
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /src

# Copy everything (including submodules)
COPY . .

# Initialize and update git submodules
RUN git submodule init && git submodule update --recursive

# Restore dependencies for the main API project
RUN dotnet restore "GeckoAPI/GeckoAPI.csproj"

# Build the project in Release mode
RUN dotnet build "GeckoAPI/GeckoAPI.csproj" -c Release -o /app/build

# ========================================
# Stage 2: Publish
# ========================================
FROM build AS publish
RUN dotnet publish "GeckoAPI/GeckoAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ========================================
# Stage 3: Runtime Image
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output from build stage
COPY --from=publish /app/publish .

# Expose port (adjust as needed)
EXPOSE 8080

# Set entrypoint
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
