FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /worker
EXPOSE 5002

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /
COPY KiotVietTimeSheet.sln .
COPY ./src /src
COPY ./tests /tests
RUN dotnet restore

WORKDIR /src/Background Worker/DomainEventProcessWorker/KiotVietTimeSheet.DomainEventProcessWorker
RUN dotnet build KiotVietTimeSheet.DomainEventProcessWorker.csproj -c Release -o /worker

FROM build AS publish
RUN dotnet publish KiotVietTimeSheet.DomainEventProcessWorker.csproj -c Release -o /worker

FROM base AS final
WORKDIR /worker
COPY --from=publish /worker .
ENTRYPOINT ["dotnet", "KiotVietTimeSheet.DomainEventProcessWorker.dll"]
