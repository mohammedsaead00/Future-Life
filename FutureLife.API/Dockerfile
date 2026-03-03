FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["FutureLife.API.csproj", "."]
RUN dotnet restore "./FutureLife.API.csproj"
COPY . .
RUN dotnet build "FutureLife.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FutureLife.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FutureLife.API.dll"]
