FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["GeckoAPI/GeckoAPI.csproj", "GeckoAPI/"]
COPY ["GeckoAPI.Model/GeckoAPI.Model.csproj", "GeckoAPI.Model/"]
COPY ["GeckoAPI.Repository/GeckoAPI.Repository.csproj", "GeckoAPI.Repository/"]
COPY ["GeckoAPI.Service/GeckoAPI.Service.csproj", "GeckoAPI.Service/"]
RUN dotnet restore "GeckoAPI/GeckoAPI.csproj"
COPY . .
WORKDIR /src/GeckoAPI
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GeckoAPI.dll"]
