﻿using Newtonsoft.Json;
using Schnell.Ai.Sdk.Configuration;
using Schnell.Ai.Sdk.DataSets;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using Schnell.Ai.Shared.Helper;

namespace Schnell.Ai.Artifacts.Csv
{
    /// <summary>
    /// Configuration for CSV-exporter
    /// </summary>
    [DataContract]
    public class CsvExporterConfiguration
    {
        /// <summary>
        /// Path to file
        /// </summary>
        [DataMember]
        [JsonProperty(Required = Required.Always)]
        public string Path { get; set; }

        /// <summary>
        /// CSV-delimiter (default is ';')
        /// </summary>
        [DataMember]
        public string Delimiter { get; set; } = ";";

        /// <summary>
        /// Write header in the CSV-file
        /// </summary>
        [DataMember]
        public bool WriteHeader { get; set; } = true;

        /// <summary>
        /// Determine if data is only appended to a file 
        /// </summary>
        [DataMember]
        public bool AppendToFile { get; set; } = true;
    }

    /// <summary>
    /// Exports data to a CSV-file
    /// </summary>
    public class CsvExporter : Schnell.Ai.Sdk.DataSets.DataSetExporter
    {
        private ConfigurationHandler<CsvExporterConfiguration> _configHandler;
        public override IConfigurationHandler ConfigurationHandler => _configHandler;

        public CsvExporter()
        {

        }

        protected override void OnBuilt()
        {
            base.OnBuilt();
            _configHandler = new ConfigurationHandler<CsvExporterConfiguration>(this.Definition);
        }

        public override async Task Export(DataSet ds, IEnumerable<IDictionary<string, object>> data)
        {

            CsvHelper.Configuration.Configuration csvConf = new CsvHelper.Configuration.Configuration(System.Globalization.CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = _configHandler.Configuration.WriteHeader,
                Delimiter = _configHandler.Configuration.Delimiter
            };

            var fileMode = System.IO.FileMode.OpenOrCreate;
            if (_configHandler.Configuration.AppendToFile)
                fileMode = System.IO.FileMode.Append;

            var recordsWritten = 0;

            using (var str = System.IO.File.Open(_configHandler.Configuration.Path, fileMode, System.IO.FileAccess.Write))
            using (var wr = new System.IO.StreamWriter(str))
            using (var writer = new CsvHelper.CsvWriter(wr, csvConf))
            {
                if (_configHandler.Configuration.WriteHeader && str.Length == 0)
                {
                    ds.FieldDefinitions.ToList().ForEach(f =>
                    {
                        writer.WriteField(f.Name);
                    });
                    writer.NextRecord();
                }

                foreach (var record in data)
                {
                    ds.FieldDefinitions.ToList().ForEach(f =>
                    {
                        if (record.ContainsKey(f.Name))
                            writer.WriteField(record[f.Name]);
                        else
                            writer.WriteField(null);
                    });
                    writer.NextRecord();
                    recordsWritten++;
                    this.Log.Progress(currentValue: recordsWritten);
                }

            }
            this.Log.ProgressCompleted();
        }
    }
}
