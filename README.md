# KiotViet TimeSheet Project

[![Quality Gate Status](https://sonarqube.citigo.com.vn/api/project_badges/measure?project=kiotviet-timesheet-service&metric=alert_status)](https://sonarqube.citigo.com.vn/dashboard?id=kiotviet-timesheet-service)

## 1. Project Achitechture

### Design, Design Parttern and Architechture

- Domain Driven Design
- Onion Achitechture
- Clean Achitechture
- Hexagon Parttern
- Repository Parttern
- CQRS Parttern
- Saga Parttern
- EventSourcing
- Mediator Parttern

## 2. Dependencies

- ServiceStack.Core 5.4.0
- ServiceStack.OrmLite.Core 5.4.0
- ServiceStack.Api.Swagger 5.4.0
- FluentValidation 8.0.100
- EntityFrameworkCore 2.1.0
- EntityFrameworkCore.SqlServer 2.1.0
- AutoMapper 7.0.1

##

docker build -f .\docker\api\Dockerfile . -t citigo-repo.kvpos.com:4434/kvdev/kv-timesheet-service:development
docker push citigo-repo.kvpos.com:4434/kvdev/kv-timesheet-service:development

##

`Cd vào thư mục src/User Interface/Api và chạy lệnh sau`
 
TimeSheetContext

`dotnet ef migrations add 'Migration name' -v -o ./Migrations/EfTimeSheetDbContext -c EfDbContext`
dotnet ef database update -c EfDbContext

EventLog Context

`dotnet ef migrations add 'Migration name' -v -o ./Migrations/EfTimeSheetIntegrationEventLogContext -c EfIntegrationEventLogContext`
dotnet ef database update -c EfIntegrationEventLogContext

## Build DomainEventProcessWorker Dockerfile

docker build -f ".\src\Background Worker\DomainEventProcessWorker\KiotVietTimeSheet.DomainEventProcessWorker\Dockerfile" . -t citigo-repo.kvpos.com:4434/kvdev/kv-timesheet-domain-event-process-worker:development
docker push citigo-repo.kvpos.com:4434/kvdev/kv-timesheet-domain-event-process-worker:development
#
