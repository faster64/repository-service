# runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.2-bionic AS base
WORKDIR /app
EXPOSE 80

# Build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /
COPY [".", "."]
#RUN dotnet restore "./src/User Interface/Worker/KiotVietTimeSheet.Worker/KiotVietTimeSheet.AuditTrailWorker.csproj"
COPY . /build
WORKDIR /build
#RUN dotnet build "./src/User Interface/Worker/KiotVietTimeSheet.Worker/KiotVietTimeSheet.AuditTrailWorker.csproj" -c Release -o /app

# Publish 
FROM build AS publish
RUN dotnet publish "./src/User Interface/Worker/KiotVietTimeSheet.Worker/KiotVietTimeSheet.AuditTrailWorker.csproj" -c Release -o /app

# Install tool for debugger
FROM build AS tools-install

RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-sos
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-trace
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-dump
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-counters

# Deployment
FROM base AS final
WORKDIR /app

# Setup debugger tools

ENV TZ=Asia/Ho_Chi_Minh
RUN echo $TZ > /etc/timezone && \
    apt-get update && apt-get install -y tzdata && \
    rm /etc/localtime && \
    ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata && \
    apt-get clean
	
RUN apt-get update \
  && apt-get upgrade -y \
  && apt-get install -y \
     file \
     lldb \
  && rm -rf /var/lib/apt/lists/*
COPY --from=tools-install /dotnetcore-tools /opt/dotnetcore-tools
COPY --from=tools-install /dotnetcore-tools /opt/dotnetcore-tools
ENV PATH="/opt/dotnetcore-tools:${PATH}"
RUN dotnet-sos install

	
COPY --from=publish /app .

ENTRYPOINT ["dotnet", "KiotVietTimeSheet.AuditTrailWorker.dll"]

arg DOCKER_IMAGE_BUILD_ARG=NULL
env DOCKER_IMAGE_BUILD_ARG=$DOCKER_IMAGE_BUILD_ARG