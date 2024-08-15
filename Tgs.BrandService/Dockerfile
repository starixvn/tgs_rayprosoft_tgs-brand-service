FROM hub.lumigame.com/base/dotnet/core/sdk:8.0 AS build
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
ARG BuildConfiguration=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.BrandService/Tgs.BrandService.csproj", "Tgs.BrandService/"]
COPY ["BrandService.Repositories/BrandService.Repositories.csproj", "BrandService.Repositories/"]
COPY ["BrandService.Core/BrandService.Core.csproj", "BrandService.Core/"]
RUN dotnet restore "Tgs.BrandService/Tgs.BrandService.csproj"
COPY . .
WORKDIR "/src/Tgs.BrandService"
RUN dotnet build "Tgs.BrandService.csproj" -c $BuildConfiguration -o /app/build

FROM build AS publish
ARG BuildConfiguration
RUN dotnet publish "Tgs.BrandService.csproj" -c $BuildConfiguration -o /app/publish

FROM hub.lumigame.com/base/dotnet/core/aspnet:8.0-alpine-24h1.1 AS base
USER app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.BrandService.dll"]