You'll need `uuid-ossp` extension for PostgreSQL (using Guid's as primary keys).
Use the psql tool in your database and run:

```
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
```

Apply all migrations:

```
dotnet ef database update --project Infrastructure\Infrastructure.csproj
```

Go to state at specific migration:

```
dotnet ef database update >>>MigrationName<<< --project Infrastructure\Infrastructure.csproj
```

Add new migration:

```
dotnet ef migrations add >>>MigrationName<<< -o Persistence/Migrations --project Infrastructure\Infrastructure.csproj --context AppDbContext
```

Remove latest migration:

```
dotnet ef migrations remove --project Infrastructure\Infrastructure.csproj
```

Reset database and remove all migrations:

 ```
 dotnet ef database update 0 --project Infrastructure\Infrastructure.csproj
 dotnet ef migrations remove --project Infrastructure\Infrastructure.csproj
 ```

Nuke database:

```
DROP SCHEMA public CASCADE;
DROP SCHEMA Identity CASCADE;
CREATE SCHEMA public;
CREATE SCHEMA Identity;
```