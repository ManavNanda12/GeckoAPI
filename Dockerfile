# Stage 1: Build and Restore
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["GeckoAPI.csproj", "./"]
RUN dotnet restore "GeckoAPI.csproj"
COPY . .
RUN dotnet build "GeckoAPI.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "GeckoAPI.csproj" -c Release -o /app/publish

# Stage 3: Runtime (your existing "base" and "final")
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
