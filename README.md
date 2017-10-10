InfluxData.Net
============
**Compatible with InfluxDB v1.3.x and Kapacitor v1.0.0 API's**

_NOTE: The **library will most probably work just as fine with newer versions** of the TICK stack as well but it hasn't been tested against them._

> InfluxData.Net is a portable .NET library to access the REST API of an [InfluxDB](https://influxdata.com/time-series-platform/influxdb/) database and [Kapacitor](https://influxdata.com/time-series-platform/kapacitor/) processing tool.

**The library supports .Net Framework v4.6.1 and .Net Standard v2.0 (which implies .Net Core 2.0).**

InfluxDB is the data storage layer in [InfluxData](https://influxdata.com/)'s [TICK stack](https://influxdata.com/get-started/#whats-the-tick-stack) which is an open-source end-to-end platform for managing time-series data at scale.

Kapacitor is a data processing engine. It can process both stream (subscribe realtime) and batch (bulk query) data from InfluxDB. Kapacitor lets you define custom logic to process alerts with dynamic thresholds, match metrics for patterns, compute statistical anomalies, etc.

Support for other TICK stack layers is also planned and will be implemented in the future when they become stable from InfluxData side.

**Original Lib**

This is a fork of [InfluxDb.Net](https://github.com/pootzko/InfluxDB.Net/), (which is in turn a fork of [InfluxDb.Net](https://github.com/ziyasal/InfluxDb.Net/)). Those NuGet libraries are only suitable for InfluxDB versions lower than v0.9.5.

**Support for older versions**

Currently older supported versions:

 - InfluxDB: v0.9.2, v0.9.6, v1.0.0, v1.3.x
 - Kapacitor: v0.10.0, v0.10.1, v1.0.0

## Table of contents

 - [Installation](#installation)
 - [Usage](#usage)
   - [API reference](#api-reference)
 - [InfluxDbStudio management tool](#influxdbstudio-management-tool)
 - [Bugs & feature requests](#bugs--feature-requests)
 - [Contributing](#contributing)
 - [License](#license)
 - [Changelog](https://github.com/pootzko/InfluxData.Net/blob/master/CHANGELOG.md)

## Installation
You can download the [InfluxData.Net Nuget](https://www.nuget.org/packages/InfluxData.Net/) package to install the latest version of InfluxData.Net Lib.

## Usage

To use InfluxData.Net InfluxDbClient you must first create an instance of `InfluxDbClient`:

```cs
var influxDbClient = new InfluxDbClient("http://yourinfluxdb.com:8086/", "username", "password", InfluxDbVersion.v_1_3);
```

Additional, optional params for InfluxDbClient are a custom `HttpClient` if you think you need control over it, and `throwOnWarning` which will throw an `InfluxDataWarningException` if the InfluxDb API returns a warning as a part of the response. That should preferably be used only for debugging purposes.

To use InfluxData.Net KapacitorClient you must first create an instance of `KapacitorClient` (Kapacitor doesn't support authentication yet, so use this overload for now):

```cs
var kapacitorClient = new KapacitorClient("http://yourkapacitor.com:9092/", KapacitorVersion.v_1_0_0);
```

Clients modules (properties of *Client* object) can then be consumed and methods for communicating with InfluxDb/Kapacitor can be consumed.

If needed, a custom HttpClient can be used for making requests. Simply pass it into the `InfluxDbClient` or `KapacitorClient` as the last (optional) parameter.

**Supported InfluxDbClient modules and API calls <a name="api-reference"></a>**

- [Client](#client-module)
  - _[WriteAsync()](#writeasync)_
  - _[QueryAsync()](#queryasync)_
  - _[QueryChunkedAsync()](#querychunkedasync)_
  - _[MultiQueryAsync()](#multiqueryasync)_
  - _[MultiQueryChunkedAsync()](#multiquerychunkedasync)_
- [Database](#database-module)
  - _[CreateDatabaseAsync()](#createdatabaseasync)_
  - _[GetDatabasesAsync()](#getdatabasesasync)_
  - _[DropDatabaseAsync()](#dropdatabaseasync)_
- [User](#user-module)
  - _[CreateUserAsync()](#createuserasync)_
  - _[GetUsersAsync()](#getusersasync)_
  - _[DropUserAsync()](#dropuserasync)_
  - _[SetPasswordAsync()](#setpasswordasync)_
  - _[GetPrivilegesAsync()](#getprivilegesasync)_
  - _[GrantAdministratorAsync()](#grantadministratorasync)_
  - _[RevokeAdministratorAsync()](#revokeadministratorasync)_
  - _[GrantPrivilegeAsync()](#grantprivilegeasync)_
  - _[RevokePrivilegeAsync()](#revokeprivilegeasync)_
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
  - _[GetTagKeysAsync()](#gettagkeysasync)_
  - _[GetTagValuesAsync()](#gettagvaluesasync)_
  - _[GetFieldKeysAsync()](#getfieldkeysasync)_
  - _[CreateBatchWriter()](#createbatchwriter)_
    - _[Start()](#bw-start)_
    - _[AddPoint()](#bw-addpoint)_
    - _[AddPoints()](#bw-addpoints)_
    - _[Stop()](#bw-stop)_
    - _[OnError()](#bw-onerror)_
    - _[SetMaxBatchSize()](#bw-setmaxbatchsize)_
- [Retention](#retention-module)
  - _[CreateRetentionPolicyAsync()](#createretentionpolicyasync)_
  - _[GetRetentionPoliciesAsync()](#getretentionpoliciesasync)_
  - _[AlterRetentionPolicyAsync()](#alterretentionpolicyasync)_
  - _[DropRetentionPolicyAsync()](#dropretentionpolicyasync)_
- [Diagnostics](#diagnostics-module)
  - _[PingAsync()](#pingasync)_
  - _[GetStatsAsync()](#getstatsasync)_
  - _[GetDiagnosticsAsync()](#getdiagnosticsasync)_
- [Helpers](#helpers)
  - _[serie.As<T>()](#serieas)_

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
var response = await influxDbClient.Client.WriteAsync(pointToWrite, "yourDbName");
```

If you would like to write multiple points at once, simply create an `IEnumerable` collection of `Point` objects and pass it into the second `WriteAsync` overload:

```cs
var response = await influxDbClient.Client.WriteAsync(pointsToWrite, "yourDbName");
```

#### QueryAsync

The `Client.QueryAsync` can be used to execute any officially supported [InfluxDb query](https://docs.influxdata.com/influxdb/v0.9/query_language/):

```cs
var query = "SELECT * FROM reading WHERE time > now() - 1h";
var response = await influxDbClient.Client.QueryAsync(query, "yourDbName"[, epochFormat = null][, ]);
```

The second `QueryAsync` overload will return the result of [multiple queries](https://docs.influxdata.com/influxdb/v0.9/guides/querying_data/) executed at once. The response will be a _flattened_ collection of multi-results series. This means that the resulting series from all queries will be extracted into a single collection. This has been implemented to make it easier on the developer in case he is querying the same measurement with different params multiple times at once.

```cs
var queries = new []
{
    "SELECT * FROM reading WHERE time > now() - 1h",
    "SELECT * FROM reading WHERE time > now() - 2h"
}
var response = await influxDbClient.Client.QueryAsync(queries, "yourDbName");
```

#### Chunked QueryAsync and MultiQueryAsync

Check the usage [here](https://github.com/pootzko/InfluxData.Net/pull/39#issuecomment-287722949).


#### Parameterized QueryAsync

With support for parameterized queries ([#61](https://github.com/pootzko/InfluxData.Net/pull/61)), InfluxDB can also be queried in the following manner:

``` cs
var serialNumber = "F2EA2B0CDFF";
var queryTemplate = "SELECT * FROM cpuTemp WHERE \"serialNumber\" = @SerialNumber";

var response = await influxDbClient.Client.QueryAsync(
    queryTemplate: queryTemplate,
    parameters: new
    {
        @SerialNumber = serialNumber
    },
    dbName: "yourDbName"
);
```



#### MultiQueryAsync

`MultiQueryAsync` also returns the result of [multiple queries](https://docs.influxdata.com/influxdb/v0.9/guides/querying_data/) executed at once. Unlike the second `QueryAsync` overload, the results *will not be flattened*. This method will return a collection of results where each result contains the series of a corresponding query.

```cs
var queries = new []
{
    "SELECT * FROM reading WHERE time > now() - 1h",
    "SELECT * FROM reading WHERE time > now() - 2h"
}
var response = await influxDbClient.Client.MultiQueryAsync(queries, "yourDbName");
```

#### MultiQueryChunkedAsync

Check the usage [here](https://github.com/pootzko/InfluxData.Net/pull/39#issuecomment-287722949).

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

### User Module

The user module can be used to [manage database users](https://docs.influxdata.com/influxdb/v0.9/administration/authentication_and_authorization/#authorization) on the InfluxDb system. The requests in the user module must be called with user credentials that have administrator privileges or authentication must be disabled on the server.

#### CreateUserAsync

Creates a new user. The user can either be created as a regular user or an administrator user by specifiy the desired value for the `isAdmin` parameter when calling the method.

To create a new user:

```cs
var response = await influxDbClient.User.CreateUserAsync("userName");
```

To create a new administrator:

```cs
var response = await influxDbClient.User.CreateUserAsync("userName", true);
```

#### GetUsersAsync

Gets a list of users for the system:

```cs
var users = await influxDbClient.User.GetUsersAsync();
```

#### DropUserAsync

Drops an existing user:

```cs
var response = await influxDbClient.User.DropUserAsync("userNameToDrop");
```

#### SetPasswordAsync

Sets a user's password:

```cs
var response = await influxDbClient.User.SetPasswordAsync("userNameToUpdate", "passwordToSet");
```

#### GetPrivilegesAsync

Gets a list of a user's granted privileges:

```cs
var grantedPrivilges = await influxDbClient.User.GetPrivilegesAsync("userNameToGetPrivilegesFor");
```

#### GrantAdministratorAsync

Grants administrator privileges to a user:

```cs
var response = await influxDbClient.User.GrantAdministratorAsync("userNameToGrantTo");
```

#### RevokeAdministratorAsync

Revokes administrator privileges from a user:

```cs
var response = await influxDbClient.User.RevokeAdministratorAsync("userNameToRevokeFrom");
```

#### GrantPrivilegeAsync

Grants the specified privilege to a user for a given database:

```cs
var response = await influxDbClient.User.GrantPrivilegeAsync("userNameToGrantTo", Privileges.Read, "databaseName");
```

#### RevokePrivilegeAsync

Revokes the specified privilege from a user for a given database:

```cs
var response = await influxDbClient.User.RevokePrivilegeAsync("userNameToRevokeFrom", Privileges.Read, "databaseName");
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

#### GetSeriesAsync

Gets [list of series](https://influxdb.com/docs/v0.9/query_language/schema_exploration.html#explore-series-with-show-series) in the database. If `measurementName` (optional) param is provided, will only return series for that measurement. `WHERE` clauses can be passed in through the optional `filters` param.

```cs
var response = await influxDbClient.Serie.GetSeriesAsync("yourDbName");
```

#### DropSeriesAsync

[Drops data points](https://docs.influxdata.com/influxdb/v0.9/query_language/database_management/#delete-series-with-drop-series) from series in database. The series itself will remain in the index.

```cs
var response = await influxDbClient.Serie.DropSeriesAsync("yourDbName", "serieNameToDrop");
```

#### GetMeasurementsAsync

Gets [list of measurements](https://influxdb.com/docs/v0.9/query_language/schema_exploration.html#explore-measurements-with-show-measurements) in the database. `WHERE` clauses can be passed in through the optional `filters` param.

```cs
var response = await influxDbClient.Serie.GetMeasurementsAsync("yourDbName");
```

#### DropMeasurementAsync

[Drops measurements](https://influxdb.com/docs/v0.9/query_language/database_management.html#delete-measurements-with-drop-measurement) from series in database. Unlike `DropSeriesAsync` it will also remove the measurement from the DB index.

```cs
var response = await influxDbClient.Serie.DropMeasurementAsync("yourDbName", "measurementNameToDrop");
```

#### GetTagKeysAsync

[Gets a list of tag keys](https://docs.influxdata.com/influxdb/v0.9/query_language/schema_exploration/#explore-tag-keys-with-show-tag-keys) for a given database and measurement.

```cs
var response = await influxDbClient.Serie.GetTagKeysAsync("yourDbName", "measurementNameToGetTagsFor");
```

#### GetTagValuesAsync

[Gets a list of tag values](https://docs.influxdata.com/influxdb/v0.9/query_language/schema_exploration/#explore-tag-values-with-show-tag-values) for a given database, measurement, and tag key.

```cs
var response = await influxDbClient.Serie.GetTagValuesAsync("yourDbName", "measurementNameToGetTagsValuesFor", "tagNameToGetValuesFor");
```

#### GetFieldKeysAsync

[Gets a list of field keys](https://docs.influxdata.com/influxdb/v0.9/query_language/schema_exploration/#explore-field-keys-with-show-field-keys) for a given database and measurement. The returned list of field keys also specify the field type per key.

```cs
var response = await influxDbClient.Serie.GetFieldKeysAsync("yourDbName", "measurementNameToGetFieldKeysFor");
```

#### CreateBatchWriter

Creates a `BatchWriter` instance which can then be shared by multiple threads/processes to be used
for batch `Point` writing in intervals (for example every five seconds). It will keep the points in-memory
for a specified interval. After the interval times out, the collection will get dequeued and "batch-written"
to InfluxDb. The `BatchWriter` will keep checking the collection for new points after each interval times
out until stopped. For thread safety, the `BatchWriter` uses the `BlockingCollection` internally.

```cs
var batchWriter = influxDbClient.Serie.CreateBatchWriter("yourDbName");
```

##### Start <a name="bw-start"></a>

Starts the async batch writing task. You can set the interval after which the points will be submitted to
the InfluxDb API (or use the default 1000ms). You can also instruct the _BatchWriter_ to not stop if the
_BatchWriter_ encounters an error by setting the _continueOnError_ to true.

```cs
batchWriter.Start(5000);
```

##### Stop <a name="bw-stop"></a>

Stops the async batch writing task.

```cs
batchWriter.Stop();
```

##### AddPoint <a name="bw-addpoint"></a>

Adds a single `Point` item to the blocking collection.

```cs
var point = new Point() { ... };
batchWriter.AddPoint(point);
```

##### AddPoints <a name="bw-addpoints"></a>

Adds a multiple `Point` items to the collection.

```cs
var points = new Point[10] { ... };
batchWriter.AddPoints(points);
```

##### OnError <a name="bw-onerror"></a>

OnError event handler. You can attach to it to handle any exceptions that might be thrown by the API.

```cs
// Attach to the event handler
batchWriter.OnError += BatchWriter_OnError;

// OnError handler method
private void BatchWriter_OnError(object sender, Exception e)
{
    // Handle the error here
}
```

##### SetMaxBatchSize <a name="bw-setmaxbatchsize"></a>

Sets the maximum size (point count) of a batch to commit to InfluxDB. If the collection currently holds more than the `maxBatchSize` points, any overflow will be commited in future requests on FIFO principle.

```cs
batchWriter.SetMaxBatchSize(10000);
```


### Retention Module

This module currently supports only a single [retention-policy](https://docs.influxdata.com/influxdb/v0.9/query_language/database_management/#retention-policy-management) action.

#### CreateRetentionPolicyAsync

This example creates the _retentionPolicyName_ policy to _1h_ and 3 copies:

```cs
var response = await influxDbClient.Retention.CreateRetentionPolicyAsync("yourDbName", "retentionPolicyName", "1h", 3);
```

#### GetRetentionPoliciesAsync

Gets a list of all retention policies in the speified database:

```cs
var response = await influxDbClient.Retention.GetRetentionPoliciesAsync("yourDbName");
```

#### AlterRetentionPolicyAsync

This example alter the _retentionPolicyName_ policy to _1h_ and 3 copies:

```cs
var response = await influxDbClient.Retention.AlterRetentionPolicyAsync("yourDbName", "retentionPolicyName", "1h", 3);
```

#### DropRetentionPolicyAsync

This example drops the _retentionPolicyName_ policycopies:

```cs
var response = await influxDbClient.Retention.AlterRetentionPolicyAsync("yourDbName", "retentionPolicyName");
```

### Diagnostics Module

This module can be used to get [diagnostics information](https://influxdb.com/docs/v0.9/administration/statistics.html) from InfluxDB server.

#### PingAsync

The `PingAsync` will return a `Pong` object which will return endpoint's InfluxDb version number, round-trip time and ping success status:

```cs
var response = await influxDbClient.Diagnostics.PingAsync();
```

#### GetStatsAsync

`GetStatsAsync` executes [SHOW STATS](https://influxdb.com/docs/v0.9/administration/statistics.html#show-stats) and parses the results into `Stats` response object.

```cs
var response = await influxDbClient.Diagnostics.GetStatsAsync();
```

#### GetDiagnosticsAsync

`GetDiagnosticsAsync` executes [SHOW DIAGNOSTICS](https://influxdb.com/docs/v0.9/administration/statistics.html#show-diagnostics) and parses the results into `Diagnostics` response object.

```cs
var response = await influxDbClient.Diagnostics.GetDiagnosticsAsync();
```

### Helpers

#### serie.As<T>() <a name="serieas"></a>

You can use it like:

```cs
var stronglyTypedCollection = serie.As<MyType>();
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

After that simply call the `DefineTaskAsync` to create a new task:

```cs
var response = await kapacitorClient.Task.DefineTaskAsync(taskParams);

```

You can also define tasks using the `DefineTemplatedTaskParams` as well. This allows you to define tasks with [template](https://docs.influxdata.com/kapacitor/v1.0/examples/template_tasks/) ID's instad of specifying the TICKscript and type directly.

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

## InfluxDbStudio management tool

For easier administration, check this neat UI management tool for InfluxDB called [InfluxDbStudio](https://github.com/CymaticLabs/InfluxDBStudio).

## Bugs & feature requests

If you encounter a bug, performance issue, a malfunction or would like a feature to be implemented, please open a new [issue](https://github.com/pootzko/InfluxData.Net/issues). If it's a bug report, please provide enough info so I could easily reproduce the issue you're experiencing - i.e. provide some sample data that's causing you issues, let me know exactly which library module you used to execute the query/request etc..

## Contributing

If you would like to contribute with a new feature, perhaps the best starting point would be to open an issue and get the conversation going. A healthy discussion might give us good ideas about how to do things even before a single line of code gets written which in turn produces better results.

Please apply your changes to the [develop branch](https://github.com/pootzko/InfluxData.Net/tree/develop) it makes it a bit easier and cleaner for me to keep everything in order. For extra points in the FLOSS hall of fame, write a few tests for your awesome contribution as well. :) Thanks for your help!

## License

Code and documentation are available according to the *MIT* License (see [LICENSE](https://github.com/pootzko/InfluxData.Net/blob/master/LICENSE)).
