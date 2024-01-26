#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

COPY /readiness.sh /opt/readiness.sh
ENV TZ=Asia/Shanghai

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Normandy.Identity.UserData.Rpc/Normandy.Identity.UserData.Rpc.csproj"

WORKDIR "/src/Normandy.Identity.UserData.Rpc"
RUN dotnet build "Normandy.Identity.UserData.Rpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Normandy.Identity.UserData.Rpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
RUN apt-get update && apt-get install -y curl
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Normandy.Identity.UserData.Rpc.dll"]