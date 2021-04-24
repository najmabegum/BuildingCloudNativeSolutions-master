#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["NationalCookies.Api/NationalCookies.Api.csproj", "NationalCookies.Api/"]
RUN dotnet restore "NationalCookies.Api/NationalCookies.Api.csproj"
COPY . .
WORKDIR "/src/NationalCookies.Api"
RUN dotnet build "NationalCookies.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NationalCookies.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NationalCookies.Api.dll"]