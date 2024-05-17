FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

ARG NIBIRU_CONNECTOR_VERSION=0.0.0

COPY . /build
WORKDIR /build/src/NibiruConnector

RUN dotnet publish -c Release -o /build/app -p:Version=$NIBIRU_CONNECTOR_VERSION -p:ContinuousIntegrationBuild=true

RUN rm -f ./app/appsettings.yaml

FROM mcr.microsoft.com/dotnet/aspnet:8.0

COPY --from=build /build/app /app

RUN mkdir /config

WORKDIR /app

ENV ASPNETCORE_HTTP_PORTS=5000
ENV ASPNETCORE_ENVIRONMENT=Production
ENV CONFIG_FILE=/config/appsettings.yaml

EXPOSE 5000

ENTRYPOINT ["dotnet", "NibiruConnector.dll"]
