using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Schnell.Ai.Sdk.Configuration;
using Schnell.Ai.Sdk.DataSets;
using MSql = MySql.Data;

namespace Schnell.Ai.Artifacts.MySql
{
    /// <summary>
    /// Configuration for SQL importer
    /// </summary>
    [DataContract]
    public class SqlExporterConfiguration
    {
        /// <summary>
        /// Connection-string
        /// </summary>
        [DataMember]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Write-Statement (SQL) with Key-Values as Parameters '@Param'
        /// </summary>
        [DataMember]
        public string WriteStatement { get; set; }
    }

    public class MySqlExporter : Sdk.DataSets.DataSetExporter
    {
        private ConfigurationHandler<SqlExporterConfiguration> _configurationHandler;
        public override IConfigurationHandler ConfigurationHandler => _configurationHandler;

        public override async Task Export(Sdk.DataSets.DataSet ds, IEnumerable<IDictionary<string, object>> data)
        {
            var recordsWritten = 0;
            using (MSql.MySqlClient.MySqlConnection connection = new MSql.MySqlClient.MySqlConnection(_configurationHandler.Configuration.ConnectionString))
            {
                DbCommand command = null;
                try
                {
                    command = new MSql.MySqlClient.MySqlCommand(_configurationHandler.Configuration.WriteStatement, connection);
                    command.Connection.Open();
                }
                catch (Exception ex)
                {
                    this.Log.Write(Sdk.Logging.LogEntry.LogType.Fatal, ex.Message);
                }
                if (command != null)
                {
                    foreach (var record in data)
                    {
                        try { 
                        var sqlParams = new List<MSql.MySqlClient.MySqlParameter>();
                        record.ToList().ForEach(d => sqlParams.Add(new MSql.MySqlClient.MySqlParameter(d.Key, d.Value)));
                        
                        await DbCommandWriter.WriteCommand(command, sqlParams.Cast<DbParameter>().ToList(), ds.FieldDefinitions);
                        recordsWritten++;
                        this.Log.Progress(currentValue: recordsWritten);
                        }
                        catch(Exception ex)
                        {
                            this.Log.Write(Sdk.Logging.LogEntry.LogType.Error, ex.Message);
                        }
                    }

                }
            }
            this.Log.ProgressCompleted();
        }

        protected override void OnBuilt()
        {
            base.OnBuilt();
            this._configurationHandler = new ConfigurationHandler<SqlExporterConfiguration>(this.Definition);
        }
    }
}
