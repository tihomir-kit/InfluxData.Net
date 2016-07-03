InfluxData.Net
============
**Compatible with InfluxDB v1.0.0-beta and Kapacitor v1.0.0-beta API's**

> InfluxData.Net is a portable .NET library to access the REST API of an [InfluxDB](https://influxdata.com/time-series-platform/influxdb/) database and [Kapacitor](https://influxdata.com/time-series-platform/kapacitor/) processing tool.

InfluxDB is the data storage layer in [InfluxData](https://influxdata.com/)'s [TICK stack](https://influxdata.com/get-started/#whats-the-tick-stack) which is an open-source end-to-end platform for managing time-series data at scale.

Kapacitor is a data processing engine. It can process both stream (subscribe realtime) and batch (bulk query) data from InfluxDB. Kapacitor lets you define custom logic to process alerts with dynamic thresholds, match metrics for patterns, compute statistical anomalies, etc.

Support for other TICK stack layers is also planned and will be implemented in the future when they become stable from InfluxData side.

**Original Lib**
This is a fork of [InfluxDb.Net](https://github.com/pootzko/InfluxDB.Net/), (which is in turn a fork of [InfluxDb.Net](https://github.com/ziyasal/InfluxDb.Net/)). Those NuGet libraries are only suitable for InfluxDB versions lower than v0.9.5.

**Support for older versions**

Currently older supported versions:

 - InfluxDB: v0.9.2, v0.9.6
 - Kapacitor: v0.10.0, v0.10.1

**Installation**
You can download the [InfluxData.Net Nuget](https://www.nuget.org/packages/InfluxData.Net/) package to install the latest version of InfluxData.Net Lib.

## Usage

To use InfluxData.Net InfluxDbClient you must first create an instance of `InfluxDbClient`:

```cs
var influxDbClient = new InfluxDbClient("http://yourinfluxdb.com:8086/", "username", "password", InfluxDbVersion.v_1_0_0);
```

To use InfluxData.Net KapacitorClient you must first create an instance of `KapacitorClient` (Kapacitor doesn't support authentication yet, so use this overload for now):

```cs
var kapacitorClient = new KapacitorClient("http://yourkapacitor.com:9092/", KapacitorVersion.v_1_0_0);
```

Clients modules (properties of *Client* object) can then be consumed and methods for communicating with InfluxDb/Kapacitor can be consumed.

**Supported InfluxDbClient modules and API calls**

- [Client](#client-module)
 - _[WriteAsync()](#writeasync)_
 - _[QueryAsync()](#queryasync)_
 - _[MultiQueryAsync()](#multiqueryasync)_
- [Database](#database-module)
 - _[CreateDatabaseAsync()](#createdatabaseasync)_
 - _[GetDatabasesAsync()](#getdatabasesasync)_
 - _[DropDatabaseAsync()](#dropdatabaseasync)_
- [ContinuousQuery](#continuous-query-module)
 - _[CreateContinuousQueryAsync()](#createcontinuousqueryasync)_
 - _[GetContinuousQueriesAsync()](#getcontinuousqueriesasync)_
 - _[DeleteContinuousQueryAsync()](#deletecontinuousqueryasync)_
 - _[BackfillAsync()](#backfillasync)_
- [Serie](#serie-module)
 - _[GetSeriesAsync()](#getseriesasync)_
 - _[DropSeriesAsync()](#dropseriesasync)_
 - _[GetMeasurementsAsync()](#getmeasurementsasync)_
 - _[DropMeasurementAsync()](#dropmeasurementasync)_
- [Retention](#retention-module)
 - _[AlterRetentionPolicyAsync()](#alterretentionpolicyasync)_
- [Diagnostics](#diagnostics-module)
 - _[PingAsync()](#pingasync)_
 - _[GetStatsAsync()](#getstatsasync)_
 - _[GetDiagnosticsAsync()](#getdiagnosticsasync)_

**Supported KapacitorClient modules and API calls**

- [Task](#task-module)
 - _[GetTaskAsync()](#gettaskasync)_
 - _[GetTasksAsync()](#gettasksasync)_
 - _[DefineTaskAsync()](#definetaskasync)_
 - _[DeleteTaskAsync()](#deletetaskasync)_
 - _[EnableTaskAsync()](#enabletaskasync)_
 - _[DisableTaskAsync()](#disabletaskasync)_

## InfluxDbClient

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
    Timestamp = DateTime.UtcNow // optional (can be set to any DateTime moment)
};
```

Point is then passed into `Client.WriteAsync` method together with the database name:

```cs
var response = await influxDbClient.Client.WriteAsync("yourDbName", pointToWrite);
```

If you would like to write multiple points at once, simply create an `IEnumerable` collection of `Point` objects and pass it into the second `WriteAsync` overload:

```cs
var response = await influxDbClient.Client.WriteAsync("yourDbName", pointsToWrite);
```

#### QueryAsync

The `Client.QueryAsync` can be used to execute any officially supported [InfluxDb query](https://docs.influxdata.com/influxdb/v0.9/query_language/):

```cs
var query = "SELECT * FROM reading WHERE time > now() - 1h";
var response = await influxDbClient.Client.QueryAsync("yourDbName", query);
```

The second `QueryAsync` overload will return the result of [multiple queries](https://docs.influxdata.com/influxdb/v0.9/guides/querying_data/) executed at once. The response will be a _flattened_ collection of multi-results series. This means that the resulting series from all queries will be extracted into a single collection. This has been implemented to make it easier on the developer in case he is querying the same measurement with different params multiple times at once.

```cs
var queries = new []
{
    "SELECT * FROM reading WHERE time > now() - 1h",
    "SELECT * FROM reading WHERE time > now() - 2h"
}
var response = await influxDbClient.Client.QueryAsync("yourDbName", queries);
```

#### MultiQueryAsync

`MultiQueryAsync` also returns the result of [multiple queries](https://docs.influxdata.com/influxdb/v0.9/guides/querying_data/) executed at once. Unlike the second `QueryAsync` overload, the results *will not be flattened*. This method will return a collection of results where each result contains the series of a corresponding query.

```cs
var queries = new []
{
    "SELECT * FROM reading WHERE time > now() - 1h",
    "SELECT * FROM reading WHERE time > now() - 2h"
}
var response = await influxDbClient.Client.MultiQueryAsync("yourDbName", queries);
```

### Database Module

The database module can be used to [manage the databases](https://docs.influxdata.com/influxdb/v0.9/query_language/database_management/) on the InfluxDb system.

#### CreateDatabaseAsync

You can create a new database in the following way:

```cs
var response = await influxDbClient.Database.CreateDatabaseAsync("newDbName");
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

### Continuous Query Module

This module can be used to manage [CQ's](https://docs.influxdata.com/influxdb/v0.9/query_language/continuous_queries/) and to backfill with aggregate data.

#### CreateContinuousQueryAsync

To create a new CQ, a `CqParams` object must first be created:

```cs
var cqParams = new CqParams()
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
var response = await influxDbClient.ContinuousQuery.CreateContinuousQueryAsync("yourDbName", cqParams);
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

#### BackfillAsync
The `ContinuousQuery.BackfillAsync` method can be used to manually calculate aggregate data for the data that was already in your DB, not only for the newly incoming data.

Similarly to `CreateContinuousQueryAsync`, a `BackfillParams` object needs to be created first:

```cs
var backfillParams = new BackfillParams()
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
    // you can also add a list of "WHERE" clause filters here
    // you can also add a list of tags to keep in the DS serie here
};
```

To understand `FillType`, please refer to the `fill()` [documentation](https://docs.influxdata.com/influxdb/v0.9/query_language/data_exploration/#the-group-by-clause-and-fill). After that, simply call `ContinuousQuery.BackfillAsync` to execute the backfill:

```cs
var response = await influxDbClient.ContinuousQuery.BackfillAsync("yourDbName", backfillParams);
```

### Serie Module

This module provides methods for listing existing DB series and measures as well as methods for removing them.

####GetSeriesAsync

Gets [list of series](https://influxdb.com/docs/v0.9/query_language/schema_exploration.html#explore-series-with-show-series) in the database. If `measurementName` (optional) param is provided, will only return series for that measurement. `WHERE` clauses can be passed in through the optional `filters` param.

```cs
var response = await influxDbClient.Serie.GetSeriesAsync("yourDbName");
```

#### DropSeriesAsync

[Drops data points](https://docs.influxdata.com/influxdb/v0.9/query_language/database_management/#delete-series-with-drop-series) from series in database. The series itself will remain in the index.

```cs
var response = await influxDbClient.Serie.DropSeriesAsync("yourDbName", "serieNameToDrop");
```

####GetMeasurementsAsync

Gets [list of measurements](https://influxdb.com/docs/v0.9/query_language/schema_exploration.html#explore-measurements-with-show-measurements) in the database. `WHERE` clauses can be passed in through the optional `filters` param.

```cs
var response = await influxDbClient.Serie.GetMeasurementsAsync("yourDbName");
```

#### DropMeasurementAsync

[Drops measurements](https://influxdb.com/docs/v0.9/query_language/database_management.html#delete-measurements-with-drop-measurement) from series in database. Unlike `DropSeriesAsync` it will also remove the measurement from the DB index.

```cs
var response = await influxDbClient.Serie.DropMeasurementAsync("yourDbName", "measurementNameToDrop");
```

### Retention Module

This module currently supports only a single [retention-policy](https://docs.influxdata.com/influxdb/v0.9/query_language/database_management/#retention-policy-management) action.

#### AlterRetentionPolicyAsync

This example alter the _retentionPolicyName_ policy to _1h_ and 3 copies:

```cs
var response = await influxDbClient.Retention.AlterRetentionPolicyAsync("yourDbName", "retentionPolicyName", "1h", 3);
```

### Diagnostics Module

This module can be used to get [diagnostics information](https://influxdb.com/docs/v0.9/administration/statistics.html) from InfluxDB server.

#### PingAsync

The `Client.PingAsync` will return a `Pong` object which will return endpoint's InfluxDb version number, round-trip time and ping success status:

```cs
var response = await influxDbClient.Client.PingAsync();
```

#### GetStatsAsync

`GetStatsAsync` executes [SHOW STATS](https://influxdb.com/docs/v0.9/administration/statistics.html#show-stats) and parses the results into `Stats` response object.

```cs
var response = await influxDbClient.Client.GetStatsAsync();
```

#### GetDiagnosticsAsync

`GetDiagnosticsAsync` executes [SHOW DIAGNOSTICS](https://influxdb.com/docs/v0.9/administration/statistics.html#show-diagnostics) and parses the results into `Diagnostics` response object.

```cs
var response = await influxDbClient.Client.GetDiagnosticsAsync();
```

## KapacitorClient

### Task Module

Can be used to do work with tasks (creation, deletion, listing, enablin, disabling..).

#### GetTaskAsync

To get a single Kapacitor task, execute the following:

```cs
var response = await kapacitorClient.Task.GetTaskAsync("taskId");
```

#### GetTasksAsync

To get all Kapacitor tasks, execute the following:

```cs
var response = await kapacitorClient.Task.GetTasksAsync();
```

#### DefineTaskAsync

To create/define a task, a `DefineTaskParams` object needs to be created first:

```cs
var taskParams = new DefineTaskParams()
{
    TaskId = "someTaskId",
    TaskType = TaskType.Stream,
    DBRPsParams = new DBRPsParams()
    {
        DbName = "yourInfluxDbName",
        RetentionPolicy = "default"
    },
    TickScript = "stream\r\n" +
                 "    |from().measurement('reading')\r\n" +
                 "    |alert()\r\n" +
                 "        .crit(lambda: \"Humidity\" < 36)\r\n" +
                 "        .log('/tmp/alerts.log')\r\n"
};

```

After that simpyl call the `DefineTaskAsync` to create a new task:

```cs
var response = await kapacitorClient.Task.DefineTaskAsync(taskParams);

```

#### DeleteTaskAsync

To delete a Kapacitor task, execute the following:

```cs
var response = await kapacitorClient.Task.DeleteTaskAsync("taskId");
```

#### EnableTaskAsync

To enable a Kapacitor task, execute the following:

```cs
var response = await kapacitorClient.Task.EnableTaskAsync("taskId");
```

#### DisableTaskAsync

To disable a Kapacitor task, execute the following:

```cs
var response = await kapacitorClient.Task.DisableTaskAsync("taskId");
```



## Bugs & feature requests

If you encounter a bug, performance issue, a malfunction or would like a feature to be implemented, please open a new [issue](https://github.com/pootzko/InfluxData.Net/issues).

## License

Code and documentation are available according to the *MIT* License (see [LICENSE](https://github.com/pootzko/InfluxData.Net/blob/master/LICENSE)).
