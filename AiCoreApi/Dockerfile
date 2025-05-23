FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
# USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

RUN apt-get update && \
    apt-get install -y python3.11 python3-pip && \
    rm -rf /var/lib/apt/lists/*

RUN apt-get update && \
    apt-get install -y ffmpeg

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AiCoreApi/AiCoreApi.csproj", "AiCoreApi/"]
RUN dotnet restore "AiCoreApi/AiCoreApi.csproj"
COPY . .
WORKDIR "/src/AiCoreApi"
RUN dotnet build "AiCoreApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AiCoreApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AiCoreApi.dll"]