FROM node:12-alpine AS nodebuilder

WORKDIR /usr/src/app/timesheet-web

RUN npm config set "//npm.fontawesome.com/:_authToken" B4F82ABE-5BFC-4AC7-96E0-6E6845EC989C
RUN npm config set "@fortawesome:registry" https://npm.fontawesome.com/

COPY timesheet-web/package*.json ./

# If you are building your code for production
# RUN npm ci --only=production
RUN npm install

COPY timesheet-web ./
COPY src ../src

RUN npm run build

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS builder

WORKDIR /app

COPY --from=nodebuilder /usr/src/app/src ./src
COPY KiotVietTimeSheet.sln ./
COPY ./tests ./tests

#RUN dotnet restore "./src/User Interface/Api/KiotVietTimeSheet.Api/KiotVietTimeSheet.Api.csproj"

WORKDIR /app/src/User Interface/Api/KiotVietTimeSheet.Api

#RUN dotnet build KiotVietTimeSheet.Api.csproj -c Release -o /app/out
RUN dotnet publish KiotVietTimeSheet.Api.csproj -c Release -o /app/out

# Install tool for debugger
FROM builder AS tools-install

RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-sos
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-trace
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-dump
RUN dotnet tool install --tool-path /dotnetcore-tools dotnet-counters


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1

ARG DOCKER_IMAGE_BUILD_ARG=NULL
ENV DOCKER_IMAGE_BUILD_ARG=$DOCKER_IMAGE_BUILD_ARG
RUN echo $DOCKER_IMAGE_BUILD_ARG

	
WORKDIR /app

RUN sed -i '/mozilla\/DST_Root_CA_X3/s//!/' /etc/ca-certificates.conf && update-ca-certificates -f

ENV TZ=Asia/Ho_Chi_Minh

RUN echo $TZ > /etc/timezone && \
    apt-get update && apt-get install -y tzdata && \
    rm /etc/localtime && \
    ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata && \
    apt-get clean

COPY --from=builder /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "KiotVietTimeSheet.Api.dll"]
