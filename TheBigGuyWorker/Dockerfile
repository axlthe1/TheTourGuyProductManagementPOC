FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TheBigGuyWorker/TheBigGuyWorker.csproj", "TheBigGuyWorker/"]
COPY ["TheTourGuy.BasicWorker/TheTourGuy.BasicWorker.csproj", "TheTourGuy.BasicWorker/"]
COPY ["TheTourGuy.Interfaces/TheTourGuy.Interfaces.csproj", "TheTourGuy.Interfaces/"]
COPY ["TheTourGuy.Models/TheTourGuy.Models.csproj", "TheTourGuy.Models/"]
RUN dotnet restore "TheBigGuyWorker/TheBigGuyWorker.csproj"
COPY . .
WORKDIR "/src/TheBigGuyWorker"
RUN dotnet build "TheBigGuyWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TheBigGuyWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheBigGuyWorker.dll"]
