FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MetaExchange.WebService/MetaExchange.WebService.csproj", "MetaExchange.WebService/"]
RUN dotnet restore "MetaExchange.WebService/MetaExchange.WebService.csproj"
COPY . .
WORKDIR "/src/MetaExchange.WebService"
RUN dotnet build "./MetaExchange.WebService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MetaExchange.WebService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MetaExchange.WebService.dll"]
