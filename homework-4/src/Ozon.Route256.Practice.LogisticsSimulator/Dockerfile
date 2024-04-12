FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Ozon.Route256.Practice.LogisticsSimulator.csproj", "src/Ozon.Route256.Practice.LogisticsSimulator/"]
RUN dotnet restore "src/Ozon.Route256.Practice.LogisticsSimulator/Ozon.Route256.Practice.LogisticsSimulator.csproj"
WORKDIR "src/Ozon.Route256.Practice.LogisticsSimulator"
COPY . .
RUN dotnet build "Ozon.Route256.Practice.LogisticsSimulator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ozon.Route256.Practice.LogisticsSimulator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozon.Route256.Practice.LogisticsSimulator.dll"]
