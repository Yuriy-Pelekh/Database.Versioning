[![Build status](https://ci.appveyor.com/api/projects/status/67m0b862xccvismj/branch/master?svg=true)](https://ci.appveyor.com/project/Yuriy-Pelekh/database-versioning/branch/master)

[![codecov](https://codecov.io/gh/Yuriy-Pelekh/Database.Versioning/branch/master/graph/badge.svg)](https://codecov.io/gh/Yuriy-Pelekh/Database.Versioning)

[NuGet](https://www.nuget.org/packages/Database.Versioning/)

# About
This project provides very straightforward and elegant way to keep track on database schema and data changes.
It is very easy to use.

## How to
After package is installed you will find the new folder called Scripts in the solution. There are two files inside the folder: `create.sql` and `update.sql`. `create.sql` contains script that creates empty database and table with database version information.
`update.sql` has dummy example how to write version changes. Before running the application go files preferences and change compile type to `Content`.

### Rules and suggestions for successful versioning
- Each version script should start from: `--##23`, where `23` is a version number. Version number is incremental.
- It's prohibited to change script in previouse versions in case it was already used in production or by other developers as these changes will not re-apply to those who already has newer version of database. Write required database changes in the next version.
- New version must have higher number than previouse. Otherwise it will be skipped.
- You can modify create.sql according to current state of database or remove database creation if you want to start versioning existing database but do not touch `Version` table. It is required to store history of database updates.
- Clear update.sql before start using it and write your own script following the example.

#### Example
In the application startup on in the installer or whenewer you need you can perform database update like this:

```
var connectionString = $"Data Source=(local);Initial Catalog={databaseName};Integrated Security=True";
var databaseManager = new DatabaseManager(connectionString);
if (!databaseManager.Exists()) {
    databaseManager.Create();
}
databaseManager.Update();
```
Enjoy!
