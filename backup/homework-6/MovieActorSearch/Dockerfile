FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MovieActorSearch/MovieActorSearch.csproj", "MovieActorSearch/"]
RUN dotnet restore "MovieActorSearch/MovieActorSearch.csproj"
COPY . .
WORKDIR "/src/MovieActorSearch"
RUN dotnet build "MovieActorSearch.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MovieActorSearch.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MovieActorSearch.dll"]
