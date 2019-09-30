# schnell.AI Artifacts (MySQL/Maria DB)

This module provides all components required for importing and exporting data from a MySql/MariaDB database.

## Installation
``` powershell
schnell.ai module-i Schnell.Ai.Artifacts.MySql 0.9.1
```

## Importer: Schnell.Ai.Artifacts.MySql.MySqlImporter

Import data from a database-source.

### Configuration

| Name             | Type    | Description                         | Default |
|------------------|---------|-------------------------------------|---------|
| ConnectionString | string* | Connection-String to database       |         |
| Query            | string* | SQL-query to use for importing data |         |

Properties with a type, ending to '*' are required.

## Exporter: Schnell.Ai.Artifacts.MySql.MySqlExporter

Exports data to a database-source.

### Configuration

| Name             | Type    | Description                      | Default |
|------------------|---------|----------------------------------|---------|
| ConnectionString | string* | Connection-String to database    |         |
| WriteStatement   | string* | SQL-statement for exporting data |         |

Properties with a type, ending to '*' are required.
