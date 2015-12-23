InfluxData.Net
============
**Compatible with InfluxDB 0.9.6 API**

> InfluxData.Net is a portable .NET library to access the REST API of an [InfluxDB](https://influxdata.com/time-series-platform/influxdb/) database. 

InfluxDB is the data storage layer in [InfluxData](https://influxdata.com/)'s [TICK stack](https://influxdata.com/get-started/#whats-the-tick-stack) which is an open-source end-to-end platform for managing time-series data at scale.

Support for other TICK stack layers is also planned and will be implemented in the future when they become stable from InfluxData side.

**Original Lib**  
This is a fork of [InfluxDb.Net](https://github.com/pootzko/InfluxDB.Net/), (which is in turn a fork of [InfluxDb.Net](https://github.com/ziyasal/InfluxDb.Net/)). Those NuGet libraries are only suitable for InfluxDB versions lower than v0.9.5.

**Installation**  
You can download the [InfluxData.Net Nuget](https://www.nuget.org/packages/InfluxData.Net/) package to install the latest version of InfluxData.Net Lib.

## Usage

To use InfluxData.Net you must first create an instance of `InfluxDbClient`:

```cs
var influxDbClient = new InfluxDbClient("http://yourinflux.com:8086/", "username", "password", InfluxDbVersion.v_0_9_6);
```

Clients modules (properties of `InfluxDbClient` object) can then be consumed and methods for communicating with InfluxDb can be consumed. 

**Supported modules**

- [Client](#client-module)
- [Database](#database-module)
- [Retention](#retention-module)
- [ContinuousQuery](#continuous-query-module)

### Client Module

Can be used to do the most basic operations against InfluxDb API.

#### WriteAsync

To write new data into InfluxDb, a Point object must be created first:

```cs
var pointToWrite = new Point()
{
    Name = "reading", // serie/measurement/table to write into
    Tags = new Dictionary<string, object>()
    {
        { "SensorId", 8 },
        { "SerialNumber", "00AF123B" }
    },
    Fields = new Dictionary<string, object>()
    {
        { "SensorState", "act" },
        { "Humidity", 431 },
        { "Temperature", 22.1 },
        { "Resistance", 34957 }
    },
    Timestamp = DateTime.UtcNow
};
```

Point is then passed into `Client.WriteAsync` method together with the database name:

```cs
var response = await influxDbClient.Client.WriteAsync("yourDbName", pointToWrite);
```

If you would like to write multiple points at once, simply create an array of `Point` objects and pass it into the second `WriteAsync` overload:

```cs
var response = await influxDbClient.Client.WriteAsync("yourDbName", pointsToWrite);
```

#### QueryAsync

The `Client.QueryAsync` can be used to execute any officially supported [InfluxDb query](https://docs.influxdata.com/influxdb/v0.9/query_language/):

```cs
var query = "SELECT * FROM reading WHERE time > now() - 1h";
var response = await influxDbClient.Client.QueryAsync("yourDbName", query);
```

#### PingAsync

The `Client.PingAsync` will return a `Pong` object which will return endpoint's InfluxDb version number, round-trip time and ping success status:

```cs
var response = await influxDbClient.Client.PingAsync();
```

### Database Module

The database module can be used to [manage the databases](https://docs.influxdata.com/influxdb/v0.9/query_language/database_management/) on the InfluxDb system.

#### CreateDatabaseAsync

You can create a new database in the following way:

```cs
var resposne = await influxDbClient.Database.CreateDatabaseAsync("newDbName");
```

#### GetDatabasesAsync

Gets a list of all databases accessible to the current user:

```cs
var response = await influxDbClient.Database.GetDatabasesAsync();
```

#### DropDatabaseAsync

Drops a database:

```cs
var response = await influxDbClient.Database.DropDatabaseAsync("dbNameToDrop");
```

#### DropSeriesAsync

[Drops all data points](https://docs.influxdata.com/influxdb/v0.9/query_language/database_management/#delete-series-with-drop-series) from series in database. The series itself will remain in the index.

```cs
var response = await influxDbClient.Database.DropSeriesAsync("yourDbName", "serieNameToDrop");
```

### Retention Module

This module currently supports only a single [retention-policy](https://docs.influxdata.com/influxdb/v0.9/query_language/database_management/#retention-policy-management) action.

#### AlterRetentionPolicy

This example alter the _retentionPolicyName_ policy to _1h_ and 3 copies.

```cs
var response = await influxDbClient.Retention.AlterRetentionPolicy("yourDbName", "retentionPolicyName", "1h", 3);
```

### Continuous Query Module

This module can be used to manage [CQ's](https://docs.influxdata.com/influxdb/v0.9/query_language/continuous_queries/) and to backfill with aggregate data.

#### CreateContinuousQueryAsync

To create a new CQ, a `ContinuousQuery` object must first be created:

```cs
var newCq = new ContinuousQuery()
{
    DbName = "yourDbName",
    CqName = "reading_minMax_5m", // CQ name
    Downsamplers = new List<string>()
    {
        "MAX(field_int) AS max_field_int",
        "MIN(field_int) AS min_field_int"
    },
    DsSerieName = "reading.minMax.5m", // new (downsample) serie name
    SourceSerieName = "reading", // source serie name to get data from
    Interval = "5m",
    FillType = FillType.Previous
    // you can also add a list of tags to keep in the DS serie here
};
```

To understand `FillType`, please refer to the `fill()` [documentation](https://docs.influxdata.com/influxdb/v0.9/query_language/data_exploration/#the-group-by-clause-and-fill). After that, simply call `ContinuousQuery.CreateContinuousQueryAsync` to create it:

```cs
var response = await influxDbClient.ContinuousQuery.CreateContinuousQueryAsync("yourDbName", newCq);
```

#### GetContinuousQueriesAsync

This will return a list of currently existing CQ's on the system:

```cs
var response = await influxDbClient.ContinuousQuery.GetContinuousQueriesAsync("yourDbName");
```

#### DeleteContinuousQueryAsync

Deletes a CQ from the database:

```cs
var response = await influxDbClient.ContinuousQuery.DeleteContinuousQueryAsync("yourDbName", "cqNameToDelete");
```

#### Backfill
The `ContinuousQuery.Backfill` method can be used to manually calculate aggregate data for the data that was already in your DB, not only for the newly incoming data. 

Similarly to `CreateContinuousQueryAsync`, a `Backfill` object needs to be created first:

```cs
var newBackfill = new Backfill()
{
    Downsamplers = new List<string>()
    {
        "MAX(field_int) AS max_field_int",
        "MIN(field_int) AS min_field_int"
    },
    DsSerieName = "reading.minMax.5m", // new (downsample) serie name
    SourceSerieName = "reading", // source serie name to get data from
    TimeFrom = DateTime.UtcNow.AddMonths(-1),
    TimeTo = DateTime.UtcNow,
    Interval = "5m",
    FillType = FillType.None
    // you can also add a list of tags to keep in the DS serie here
};
```

To understand `FillType`, please refer to the `fill()` [documentation](https://docs.influxdata.com/influxdb/v0.9/query_language/data_exploration/#the-group-by-clause-and-fill). After that, simply call `ContinuousQuery.Backfill` to execute the backfill:

```cs
var response = await influxDbClient.ContinuousQuery.Backfill("yourDbName", newBackfill);
```

## Bugs & feature requests

If you encounter a bug, performance issue, a malfunction or would like a feature to be implemented, please open a new [issue](https://github.com/pootzko/InfluxData.Net/issues).

## License

Code and documentation are available according to the *MIT* License (see [LICENSE](https://github.com/pootzko/InfluxData.Net/blob/master/LICENSE)).
