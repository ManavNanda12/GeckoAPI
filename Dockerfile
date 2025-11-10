# Stage 1: Build and Restore
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all project files so restore works for all dependencies
COPY ["GeckoAPI/GeckoAPI.csproj", "GeckoAPI/"]
COPY ["GeckoAPI.Model/GeckoAPI.Model.csproj", "GeckoAPI.Model/"]
COPY ["GeckoAPI.Repository/GeckoAPI.Repository.csproj", "GeckoAPI.Repository/"]
COPY ["GeckoAPI.Service/GeckoAPI.Service.csproj", "GeckoAPI.Service/"]

# Copy solution file if you have one
COPY ["GeckoAPI.sln", "./"]

# Restore dependencies for all projects
RUN dotnet restore "GeckoAPI/GeckoAPI.csproj"

# Copy the rest of the source code
COPY . .

# Build the main API project
WORKDIR /src/GeckoAPI
RUN dotnet build "GeckoAPI.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
WORKDIR /src/GeckoAPI
RUN dotnet publish "GeckoAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
