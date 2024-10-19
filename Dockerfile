FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TheTourGuy.ProductSearcherApi/TheTourGuy.ProductSearcherApi.csproj", "TheTourGuy.ProductSearcherApi/"]
COPY ["TheTourGuy.Models/TheTourGuy.Models.csproj", "TheTourGuy.Models/"]
COPY ["TheTourGuy.DTO/TheTourGuy.DTO.csproj", "TheTourGuy.DTO/"]
RUN dotnet restore "TheTourGuy.ProductSearcherApi/TheTourGuy.ProductSearcherApi.csproj"
COPY . .
WORKDIR "/src/TheTourGuy.ProductSearcherApi"
RUN dotnet build "TheTourGuy.ProductSearcherApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TheTourGuy.ProductSearcherApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheTourGuy.ProductSearcherApi.dll"]
