FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
COPY ./docker-reference/sources.list /etc/apt/sources.list
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends wget gdb lldb
RUN wget https://packages.microsoft.com/repos/microsoft-debian-buster-prod/pool/main/p/procdump/procdump_1.1.1-220_amd64.deb -O procdump.deb \
    && dpkg -i procdump.deb \
    && rm procdump.deb

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AutoDump.csproj", ""]
RUN dotnet restore "./AutoDump.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AutoDump.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutoDump.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY entrypoint.sh ./entrypoint.sh
COPY --from=publish /app/publish .
RUN chmod +x ./entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh", "/app/AutoDump.dll"]