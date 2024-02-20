FROM mcr.microsoft.com/dotnet/aspnet:8.0.1-bookworm-slim-amd64 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.101-bookworm-slim-amd64 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ChristopherBriddock.WorkerService.Email/ChristopherBriddock.WorkerService.Email.csproj", "ChristopherBriddock.WorkerService.Email/"]
COPY ["ChristopherBriddock.Service.Common/ChristopherBriddock.Service.Common.csproj", "ChristopherBriddock.Service.Common/"]
RUN dotnet restore "./ChristopherBriddock.WorkerService.Email/./ChristopherBriddock.WorkerService.Email.csproj"
COPY . .
WORKDIR "/src/ChristopherBriddock.WorkerService.Email"
RUN dotnet build "./ChristopherBriddock.WorkerService.Email.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ChristopherBriddock.WorkerService.Email.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChristopherBriddock.WorkerService.Email.dll"]