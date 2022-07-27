#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BuildConfiguration=Release
RUN --mount=type=secret,id=nuget,dst=/pat cp /pat ~/.nuget/NuGet/NuGet.Config
RUN dotnet nuget remove source 'Microsoft Visual Studio Offline Packages' || true
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

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.BrandService.dll"]