FROM mcr.microsoft.com/dotnet/sdk:8.0 as build

COPY . /build
WORKDIR /build

RUN dotnet publish -c Release -o ./app

FROM mcr.microsoft.com/dotnet/runtime:8.0

COPY --from=build /build/app /app

WORKDIR /app

ENTRYPOINT ["dotnet", "NibiruConnector.dll"]
