# Change Log

## [v8.0.0](https://www.nuget.org/packages/InfluxData.Net/8.0.0) (2017-10-07)

- added a `IEnumerable<Serie>` extension method for converting ubiquitous series to a strongly typed collection [\#60](https://github.com/pootzko/InfluxData.Net/issues/60) ([joakimhew](https://github.com/joakimhew))
- added support for parameterized queries [\#61](https://github.com/pootzko/InfluxData.Net/issues/61) ([joakimhew](https://github.com/joakimhew))
- added support for sending query data as form data of the request (this is the default now), rather then sending it through Uri params [\#53](https://github.com/pootzko/InfluxData.Net/issues/53) ([vgt](https://github.com/vgt))
- fixed tag and field key and value escaping in the latest version of InfluxDB [\#49](https://github.com/pootzko/InfluxData.Net/issues/49)
- added support for InfluxDB v1.3.x

## [v7.0.3](https://www.nuget.org/packages/InfluxData.Net/7.0.3) (2017-08-19)

- added support for .Net Standard v2.0 [\#50](https://github.com/pootzko/InfluxData.Net/issues/50) ([hanssens](https://github.com/hanssens))

## [v7.0.1](https://www.nuget.org/packages/InfluxData.Net/7.0.1) (2017-07-03)

- replaced custom HttpUtility with Uri.EscapeDataString [\#44](https://github.com/pootzko/InfluxData.Net/issues/44) which allows support of additional characters in queries

## [v7.0.0](https://www.nuget.org/packages/InfluxData.Net/7.0.0) (2017-04-23)

This update brought some breaking API changes to the BasicClientModule (.Client) and ClientModuleBase classes. "dbName" and "query" have swapped places so please ensure you check and update your code (since both params are of string type) before deploying this to production. Sorry for not keeping the old signatures, I do this in my spare time and such amount of backwards compatibility would simply require more of my free time to maintain it properly which I don't have at this stage.

List of changes:
- removed user/pass validation from InfluxDbClient instantiation [\#42](https://github.com/pootzko/InfluxData.Net/issues/42)
- added epochFormat support in (Base) Client module [\#40](https://github.com/pootzko/InfluxData.Net/issues/40) ([semigroupoid](https://github.com/semigroupoid))
- added basic chunked responses support [\#39](https://github.com/pootzko/InfluxData.Net/issues/39)
- replaced custom HttpUtility with Uri.EscapeDataString [\#44](https://github.com/pootzko/InfluxData.Net/issues/44) which allows support of additional characters in queries

## [v6.0.1](https://www.nuget.org/packages/InfluxData.Net/6.0.1) (2017-20-03)

- batched interval writing maxPointsPerBatch option
- bugfix when fetching user list when no users exist

## [v5.0.1](https://www.nuget.org/packages/InfluxData.Net/5.0.1)

- added support for InfluxDB and Kapacitor v1.0.0-beta
- performance improvements (HttpClient)

## [v4.2.2](https://www.nuget.org/packages/InfluxData.Net/4.2.2)

- implemented ConfigureAwait for async methods

## [v4.0.3](https://www.nuget.org/packages/InfluxData.Net/4.0.3)

- serie module
- diagnostics module
- bugfixes

## [v3.0.3](https://www.nuget.org/packages/InfluxData.Net/3.0.3)

- saving points custom timestamp bugfix
- added Json.NET to nuget dependencies

