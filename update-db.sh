
dotnet ef migrations add AddEmployeeEntity -v -o ./Migrations

dotnet ef database update
