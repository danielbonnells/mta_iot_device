FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ESP32_MTA_Feed.csproj", "."]
RUN dotnet restore "ESP32_MTA_Feed.csproj"
COPY . .
RUN dotnet publish "ESP32_MTA_Feed.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ESP32_MTA_Feed.dll"]