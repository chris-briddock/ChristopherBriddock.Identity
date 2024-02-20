FROM mcr.microsoft.com/dotnet/aspnet:8.0.1-bookworm-slim-amd64 AS base
USER app
WORKDIR /app

EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0.101-bookworm-slim-amd64 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChristopherBriddock.Service.Identity/ChristopherBriddock.Service.Identity.csproj", "ChristopherBriddock.Service.Identity/"]
COPY ["ChristopherBriddock.Service.Common/ChristopherBriddock.Service.Common.csproj", "ChristopherBriddock.Service.Common/"]
RUN dotnet restore "./ChristopherBriddock.Service.Identity/./ChristopherBriddock.Service.Identity.csproj"
COPY . .
WORKDIR "/src/ChristopherBriddock.Service.Identity"
RUN dotnet build "./ChristopherBriddock.Service.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChristopherBriddock.Service.Identity.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChristopherBriddock.Service.Identity.dll"]