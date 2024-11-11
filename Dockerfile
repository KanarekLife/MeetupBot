﻿FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
RUN mkdir /db
RUN chown $APP_UID:$APP_UID /db
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MeetupBot.csproj", "./"]
RUN dotnet restore "MeetupBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "MeetupBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MeetupBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeetupBot.dll"]
