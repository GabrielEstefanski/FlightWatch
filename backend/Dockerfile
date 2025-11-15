FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/FlightWatch.Api/FlightWatch.Api.csproj", "src/FlightWatch.Api/"]
COPY ["src/FlightWatch.Application/FlightWatch.Application.csproj", "src/FlightWatch.Application/"]
COPY ["src/FlightWatch.Domain/FlightWatch.Domain.csproj", "src/FlightWatch.Domain/"]
COPY ["src/FlightWatch.Infrastructure/FlightWatch.Infrastructure.csproj", "src/FlightWatch.Infrastructure/"]

RUN dotnet restore "src/FlightWatch.Api/FlightWatch.Api.csproj"

COPY src/ ./src/

WORKDIR "/src/src/FlightWatch.Api"
RUN dotnet build "FlightWatch.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlightWatch.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "FlightWatch.Api.dll"]

