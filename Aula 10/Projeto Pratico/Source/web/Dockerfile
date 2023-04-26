# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY postcard/ .

RUN dotnet restore

# copy and publish app and libraries
RUN dotnet publish -c Release -o /app

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-bullseye-slim-amd64
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "postcard.dll"]