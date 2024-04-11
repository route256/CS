FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src
COPY ["Ozon.Route256.Practice.CustomerService.Application.csproj", "src/Ozon.Route256.Practice.CustomerService.Application/"]
RUN dotnet restore "src/Ozon.Route256.Practice.CustomerService.Application/Ozon.Route256.Practice.CustomerService.Application.csproj"
WORKDIR "src/Ozon.Route256.Practice.CustomerService.Application"
COPY . .
RUN dotnet build "Ozon.Route256.Practice.CustomerService.Application.csproj" -c Release -o /app/build

WORKDIR /src
COPY ["Ozon.Route256.Practice.CustomerService.Domain.csproj", "src/Ozon.Route256.Practice.CustomerService.Domain/"]
RUN dotnet restore "src/Ozon.Route256.Practice.CustomerService.Domain/Ozon.Route256.Practice.CustomerService.Domain.csproj"
WORKDIR "src/Ozon.Route256.Practice.CustomerService.Domain"
COPY . .
RUN dotnet build "Ozon.Route256.Practice.CustomerService.Domain.csproj" -c Release -o /app/build

WORKDIR /src
COPY ["Ozon.Route256.Practice.CustomerService.Infrastructure.csproj", "src/Ozon.Route256.Practice.CustomerService.Infrastructure/"]
RUN dotnet restore "src/Ozon.Route256.Practice.CustomerService.Infrastructure/Ozon.Route256.Practice.CustomerService.Infrastructure.csproj"
WORKDIR "src/Ozon.Route256.Practice.CustomerService.Infrastructure"
COPY . .
RUN dotnet build "Ozon.Route256.Practice.CustomerService.Infrastructure.csproj" -c Release -o /app/build

WORKDIR /src
COPY ["Ozon.Route256.Practice.CustomerService.Observation.csproj", "src/Ozon.Route256.Practice.CustomerService.Observation/"]
RUN dotnet restore "src/Ozon.Route256.Practice.CustomerService.Observation/Ozon.Route256.Practice.CustomerService.Observation.csproj"
WORKDIR "src/Ozon.Route256.Practice.CustomerService.Observation"
COPY . .
RUN dotnet build "Ozon.Route256.Practice.CustomerService.Observation.csproj" -c Release -o /app/build

WORKDIR /src
COPY ["Ozon.Route256.Practice.CustomerService.csproj", "src/Ozon.Route256.Practice.CustomerService/"]
RUN dotnet restore "src/Ozon.Route256.Practice.CustomerService/Ozon.Route256.Practice.CustomerService.csproj"
WORKDIR "src/Ozon.Route256.Practice.CustomerService"
COPY . .
RUN dotnet build "Ozon.Route256.Practice.CustomerService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ozon.Route256.Practice.CustomerService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ozon.Route256.Practice.CustomerService.dll"]
