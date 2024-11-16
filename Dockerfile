FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG TARGETPLATFORM
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY MeetupBot.csproj .
RUN --mount=type=cache,id=nuget=$TARGETPLATFORM,target=/root/.nuget/packages dotnet restore MeetupBot.csproj -a $TARGETARCH
COPY . .
RUN --mount=type=cache,id=nuget=$TARGETPLATFORM,target=/root/.nuget/packages dotnet publish MeetupBot.csproj -a $TARGETARCH --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS base

RUN mkdir /db
RUN chown $APP_UID:$APP_UID /db
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
USER $APP_UID
WORKDIR /app

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MeetupBot.dll"]
