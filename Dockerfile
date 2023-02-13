FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

COPY . /build
WORKDIR /build

RUN dotnet publish -c Release -o ./app

FROM mcr.microsoft.com/dotnet/runtime:7.0

COPY --from=build /build/app /app

WORKDIR /app

ENV DISCORD_TOKEN=""
ENV DISCORD_GUILD_ID=""
ENV RCON_IP_ADDRESS=""
ENV RCON_PORT=""
ENV RCON_PASSWORD=""

ENTRYPOINT ["dotnet", "NibiruConnector.dll"]
