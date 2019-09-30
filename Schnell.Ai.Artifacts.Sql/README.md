# schnell.AI Artifacts (Microsoft SQL Server) *EXPERIMENTAL*

This module provides all components required for importing and exporting data from a Microsoft SQL Server database.

## Installation
This module is experimental and not available via module-installation yet. 

## Importer: Schnell.Ai.Artifacts.Sql.SqlImporter

Import data from a database-source.

### Configuration

| Name             | Type    | Description                         | Default |
|------------------|---------|-------------------------------------|---------|
| ConnectionString | string* | Connection-String to database       |         |
| Query            | string* | SQL-query to use for importing data |         |

Properties with a type, ending to '*' are required.

## Exporter: Schnell.Ai.Artifacts.Sql.SqlExporter

Exports data to a database-source.

### Configuration

| Name             | Type    | Description                      | Default |
|------------------|---------|----------------------------------|---------|
| ConnectionString | string* | Connection-String to database    |         |
| WriteStatement   | string* | SQL-statement for exporting data |         |

Properties with a type, ending to '*' are required.
