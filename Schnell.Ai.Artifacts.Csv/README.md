# schnell.AI Artifacts (CSV)

This module provides all components required for importing and exporting CSV-data.

## Installation
``` powershell
schnell.ai module-i Schnell.Ai.Artifacts.Csv 0.9.1
```

## Importer: Schnell.Ai.Artifacts.Csv.CsvImporter

Import data from a CSV-file-source.

### Configuration

| Name      | Type    | Description                              | Default |
|-----------|---------|------------------------------------------|---------|
| Path      | string* | Path to CSV-file                         |         |
| Delimiter | string  | Delimiter-character                      | ;       |
| HasHeader | boolean | Determines, if the CSV-file has a header | false   |

Properties with a type, ending to '*' are required.

## Exporter: Schnell.Ai.Artifacts.Csv.CsvExporter

Exports data to a CSV-file-source.

### Configuration

| Name         | Type    | Description                                       | Default |
|--------------|---------|---------------------------------------------------|---------|
| Path         | string* | Path to CSV-file                                  |         |
| Delimiter    | string  | Delimiter-character                               | ;       |
| WriteHeader  | boolean | Write header in the CSV-file                      | true    |
| AppendToFile | boolean | Determine, if data is appendet to a existing file | true    |

Properties with a type, ending to '*' are required.
