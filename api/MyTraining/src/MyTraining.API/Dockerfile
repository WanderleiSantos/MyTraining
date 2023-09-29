FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MyTraining.API/MyTraining.API.csproj", "MyTraining.API/"]
RUN dotnet restore "MyTraining.API/MyTraining.API.csproj"
COPY . .
WORKDIR "/src/MyTraining.API"
RUN dotnet build "MyTraining.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyTraining.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyTraining.API.dll"]
