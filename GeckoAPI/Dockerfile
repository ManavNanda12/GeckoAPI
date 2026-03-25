FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all project files
COPY ["GeckoAPI/GeckoAPI.csproj", "GeckoAPI/"]
COPY ["GeckoAPI.Model/GeckoAPI.Model.csproj", "GeckoAPI.Model/"]
COPY ["GeckoAPI.Repository/GeckoAPI.Repository.csproj", "GeckoAPI.Repository/"]
COPY ["GeckoAPI.Service/GeckoAPI.Service.csproj", "GeckoAPI.Service/"]

RUN dotnet restore "GeckoAPI/GeckoAPI.csproj"

# Copy everything
COPY . .

WORKDIR "/src/GeckoAPI"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]