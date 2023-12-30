FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["aspnet3.csproj", "."]
RUN dotnet restore "./aspnet3.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./aspnet3.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./aspnet3.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "aspnet3.dll"]