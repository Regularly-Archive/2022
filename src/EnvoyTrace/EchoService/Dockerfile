FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 8083

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["EchoService.csproj", ""]
RUN dotnet restore "./EchoService.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "EchoService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "EchoService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EchoService.dll"]