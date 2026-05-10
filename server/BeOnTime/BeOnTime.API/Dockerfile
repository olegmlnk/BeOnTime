FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BeOnTime.API/BeOnTime.API.csproj", "BeOnTime.API/"]
COPY ["BeOnTime.Application/BeOnTime.Application.csproj", "BeOnTime.Application/"]
COPY ["BeOnTime.Core/BeOnTime.Core.csproj", "BeOnTime.Core/"]
COPY ["BeOnTime.Infrastructure/BeOnTime.Infrastructure.csproj", "BeOnTime.Infrastructure/"]
RUN dotnet restore "BeOnTime.API/BeOnTime.API.csproj"
COPY . .
WORKDIR "/src/BeOnTime.API"
RUN dotnet build "./BeOnTime.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BeOnTime.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BeOnTime.API.dll"]
