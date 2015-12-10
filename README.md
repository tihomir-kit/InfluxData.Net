InfluxData.Net
============
####Update for 0.9.6 API changes

>[InfluxData platform](https://influxdata.com/) An open-source end-to-end platform for managing time-series data at scale.

A Portable .NET library to access the REST API of a [InfluxData](https://influxdata.com/) TICK stack.

**Original Lib**  
This is a fork of [InfluxDb.Net](https://github.com/pootzko/InfluxDB.Net/) (which is a fork of [InfluxDb.Net](https://github.com/ziyasal/InfluxDb.Net/)) NuGet library which only for InfluxDB.

**Installation**  
I did not create a NuGet for this Lib yet so you will have to download the .sln, build it in release and add it to your project. Unit tests work, and the Influx API calls that were implemented so far seem to work fine. I will try to put out a NuGet package as well asap.

####List of supported methods (More documentation available soon)
- [Ping](#ping)
- [Version](#version)
- [CreateDatabase](#create-database)
- [DeleteDatabase](#delete-database)
- [DescribeDatabases](#describe-databases)
- [Write](#write)
- [Query](#query)
- CreateClusterAdmin(User user);
- DeleteClusterAdmin(string name);
- DescribeClusterAdmins();
- UpdateClusterAdmin(User user, string name);
- CreateDatabaseUser(string database, User user);
- DeleteDatabaseUser(string database, string name);
- DescribeDatabaseUsers(String database);
- UpdateDatabaseUser(string database, User user, string name);
- AuthenticateDatabaseUser(string database, string user, string password);
- GetContinuousQueries(String database);
- DeleteContinuousQuery(string database, int id);
- DeleteSeries(string database, string name);
- ForceRaftCompaction();
- Interfaces();
- Sync();
- ListServers();
- RemoveServers(int id);
- CreateShard(Shard shard);
- GetShards();
- DropShard(int id, Shard.Member servers);
- GetShardSpaces();
- DropShardSpace(string database, string name);
- CreateShardSpace(string database, ShardSpace shardSpace);

## Ping
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
  Pong pong =await _client.PingAsync();
```
## Version
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
  string version =await  _client.VersionAsync();
```
## Create Database
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
 InfluxDbApiCreateResponse response =await  _client.CreateDatabaseAsync("MyDb");
 //Or
 InfluxDbApiCreateResponse response = await _client.CreateDatabaseAsync(new DatabaseConfiguration
            {
                Name = "MyDb"
            });
```
## Delete Database
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
InfluxDbApiDeleteResponse deleteResponse = await _client.DeleteDatabaseAsync("MyDb");
```
## Describe Databases
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
List<Database> databases = await _client.DescribeDatabasesAsync();
```
## Write
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
Serie serie = new Serie.Builder("testSeries")
                .Columns("value1", "value2")
                .Values(DateTime.Now.Millisecond, 5)
                .Build();
InfluxDbApiResponse writeResponse =await _client.WriteAsync("MyDb", TimeUnit.Milliseconds, serie);
```

## Query
```csharp
var _client = new InfluxDb("http://...:8086", "root", "root");
 List<Serie> series = await _client.QueryAsync("MyDb", "select * from testSeries"), TimeUnit.Milliseconds);
```

##Bugs
If you encounter a bug, performance issue, or malfunction, please add an [Issue](https://github.com/pootzko/InfluxData.Net/issues) with steps on how to reproduce the problem.

##TODO
- Add more tests
- Add more documentation

##License

Code and documentation are available according to the *MIT* License (see [LICENSE](https://github.com/pootzko/InfluxData.Net/blob/master/LICENSE)).
