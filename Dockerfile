# -------------------- Base runtime image --------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ARG APP_UID=1000
USER $APP_UID
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# -------------------- Build stage --------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files so references work
COPY ["ToksikApp.API/ToksikApp.API.csproj", "ToksikApp.API/"]
COPY ["ToksikApp.Contracts/ToksikApp.Contracts.csproj", "ToksikApp.Contracts/"]

RUN dotnet restore "ToksikApp.API/ToksikApp.API.csproj"

# Copy everything else
COPY . .
WORKDIR "/src/ToksikApp.API"
RUN dotnet build "ToksikApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -------------------- Publish stage --------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ToksikApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------------------- Final runtime image --------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToksikApp.API.dll"]