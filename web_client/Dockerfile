FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["web_client/web_client.csproj", "web_client/"]
RUN dotnet restore "./web_client/web_client.csproj"
COPY . .
WORKDIR "/src/web_client"
RUN dotnet build "web_client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "web_client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "web_client.dll"]

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
#Copy from Environment(build/publish)
COPY --from=publish app/publish/wwwroot . 
COPY ["web_client/nginx.conf", "/etc/nginx/nginx.conf"]

