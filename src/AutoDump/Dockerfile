FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
RUN apt-get update && apt-get install -y --no-install-recommends wget gdb lldb
RUN wget https://packages.microsoft.com/repos/microsoft-debian-buster-prod/pool/main/p/procdump/procdump_1.1.1-220_amd64.deb -O procdump.deb \
    && dpkg -i procdump.deb \
    && rm procdump.deb
COPY ./docker-reference/sources.list /etc/apt/sources.list

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
COPY --from=publish /app/publish .
RUN echo "#!/bin/bash \n\
    procdump -M 200 -w dotnet & \n\
    dotnet \$1 \n\" > ./entrypoint.sh
RUN chmod +x ./entrypoint.sh
ENTRYPOINT ["./entrypoint.sh", "AutoDump.dll"]