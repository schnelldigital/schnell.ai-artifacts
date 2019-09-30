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

namespace Schnell.Ai.Artifacts.Sql
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

    public class SqlExporter : Sdk.DataSets.DataSetExporter
    {
        private ConfigurationHandler<SqlExporterConfiguration> _configurationHandler;
        public override IConfigurationHandler ConfigurationHandler => _configurationHandler;

        public override async Task Export(Sdk.DataSets.DataSet ds, IEnumerable<IDictionary<string, object>> data)
        {
            var recordsWritten = 0;
            using (SqlConnection connection = new SqlConnection(_configurationHandler.Configuration.ConnectionString))
            {
                DbCommand command = null;
                try
                {
                    command = new SqlCommand(_configurationHandler.Configuration.WriteStatement, connection);
                    command.Connection.Open();
                }
                catch (Exception ex)
                {
                    this.Log.Write(Sdk.Logging.LogEntry.LogType.Fatal, ex.Message);
                }
                if (command != null)
                {
                    var sqlParams = new List<SqlParameter>();
                    foreach(var record in data)
                    {
                        record.ToList().ForEach(d => sqlParams.Add(new SqlParameter(d.Key, d.Value)));    
                        await DbCommandWriter.WriteCommand(command, sqlParams.Cast<DbParameter>().ToList(), ds.FieldDefinitions);
                        recordsWritten++;
                        this.Log.Progress(currentValue: recordsWritten);
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
