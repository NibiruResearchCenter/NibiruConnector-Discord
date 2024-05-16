FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

COPY . /build
WORKDIR /build

RUN dotnet publish -c Release -o ./app

FROM mcr.microsoft.com/dotnet/aspnet:8.0

COPY --from=build /build/app /app

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:5000
ENV DOTNET_ENV=Production

EXPOSE 5000

ENTRYPOINT ["dotnet", "NibiruConnector.dll"]
