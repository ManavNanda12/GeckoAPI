# ========================================
# Stage 1: Restore & Build
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy the solution file
COPY *.sln ./

# Copy all .csproj files (maintain directory structure)
COPY GeckoAPI/*.csproj ./GeckoAPI/
COPY Model/*.csproj ./Model/
COPY Common/*.csproj ./Common/
COPY Repository/*.csproj ./Repository/
COPY Service/*.csproj ./Service/

# Restore all projects
RUN dotnet restore

# Copy everything else (all source files)
COPY . .

# Build the main project
RUN dotnet build -c Release -o /app/build

# ========================================
# Stage 2: Publish
# ========================================
FROM build AS publish
RUN dotnet publish ./GeckoAPI/GeckoAPI.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========================================
# Stage 3: Runtime
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
