# base
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
COPY ./docker-reference/sources.list /etc/apt/sources.list
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends wget gdb lldb curl
RUN wget https://github.com/Sysinternals/ProcDump-for-Linux/releases/download/1.4/procdump_1.4-13733_amd64.deb -O procdump.deb \
    && dpkg -i procdump.deb \
    && rm procdump.deb

# build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AutoDump.csproj", ""]
RUN dotnet restore "./AutoDump.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AutoDump.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AutoDump.csproj" -c Release -o /app/publish

# final 
FROM base AS final
WORKDIR /app
COPY entrypoint.sh ./entrypoint.sh
COPY --from=publish /app/publish .
RUN chmod +x ./entrypoint.sh
ENTRYPOINT ["/app/entrypoint.sh", "/app/AutoDump.dll"]