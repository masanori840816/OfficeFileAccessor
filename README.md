# OfficeFileAccessor
## About
This is an ASP.NET Core application to access docx and xlsx files.

### Environtments
* .NET ver.9.0.100
* Node.js ver.22.11.0
* React


## Environment variables

|Name|Value|
|--|--|
|ConnectionStrings__OfficeFileAccessor|Host=localhost;Port=5432;Database=office_file_accessor;Username={User Name};Password={Password};|
|Jwt__Key|your_jwt_key|

## DB Migrations
### Create a migration file
```
dotnet ef migrations add CreateApplicationUser
```

### Update the database
```
dotnet ef database update
```

### Build
#### For Windows
```
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## License
MIT