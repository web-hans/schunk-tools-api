# ── Schunk Tools API – Multi-Stage Build ──────────────────────────────────────

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["SchunkToolsApi.csproj", "."]
RUN dotnet restore "SchunkToolsApi.csproj"

COPY . .
RUN dotnet publish "SchunkToolsApi.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# Stage 2: Runtime (deutlich kleineres Image)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SchunkToolsApi.dll"]
