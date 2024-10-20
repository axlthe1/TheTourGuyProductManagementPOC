FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SomeOtherGuyWorker/SomeOtherGuyWorker.csproj", "SomeOtherGuyWorker/"]
COPY ["TheTourGuy.Models/TheTourGuy.Models.csproj", "TheTourGuy.Models/"]
COPY ["TheTourGuy.Interfaces/TheTourGuy.Interfaces.csproj", "TheTourGuy.Interfaces/"]
COPY ["TheTourGuy.BasicWorker/TheTourGuy.BasicWorker.csproj", "TheTourGuy.BasicWorker/"]
RUN dotnet restore "SomeOtherGuyWorker/SomeOtherGuyWorker.csproj"
COPY . .
WORKDIR "/src/SomeOtherGuyWorker"
RUN dotnet build "SomeOtherGuyWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SomeOtherGuyWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SomeOtherGuyWorker.dll"]
