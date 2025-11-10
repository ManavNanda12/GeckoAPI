FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Debug: Show what we have
RUN echo "===== Directory Structure =====" && \
    find . -maxdepth 3 -type f -name "*.sln" -o -name "*.csproj" | head -20

# Find and restore the solution
RUN dotnet restore "$(find . -maxdepth 2 -name '*.sln' | head -1)"

# Build
RUN dotnet build -c Release

# Find the main API project and publish
RUN dotnet publish "$(find . -maxdepth 2 -path '*/GeckoAPI.csproj' | head -1)" \
    -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
