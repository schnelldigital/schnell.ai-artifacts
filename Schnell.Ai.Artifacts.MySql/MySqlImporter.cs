using Schnell.Ai.Sdk.Configuration;
using Schnell.Ai.Sdk.DataSets;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using MSql = MySql.Data;

namespace Schnell.Ai.Artifacts.MySql
{
    /// <summary>
    /// Configuration for MySQL importer
    /// </summary>
    [DataContract]
    public class SqlImporterConfiguration
    {
        /// <summary>
        /// Connection-string
        /// </summary>
        [DataMember]
        public string ConnectionString { get; set; }

        /// <summary>
        /// Query (SQL-Statement)
        /// </summary>
        [DataMember]
        public string Query { get; set; }
    }

    public class MySqlImporter : Sdk.DataSets.DataSetImporter
    {
        private ConfigurationHandler<SqlImporterConfiguration> _configurationHandler;
        public override IConfigurationHandler ConfigurationHandler => _configurationHandler;

        public override async Task<IEnumerable<IDictionary<string, object>>> Import(DataSet ds)
        {

            MSql.MySqlClient.MySqlConnection connection = new MSql.MySqlClient.MySqlConnection(_configurationHandler.Configuration.ConnectionString);
            
            DbCommand command = null;
            try
            {
                this.Log.Write(Sdk.Logging.LogEntry.LogType.Debug, $"Build connection with string: {_configurationHandler.Configuration.ConnectionString}");
                connection.Open();
                this.Log.Write(Sdk.Logging.LogEntry.LogType.Info, $"Connection to MySQL/MariaDB is {connection.State}");                    
                command = new MSql.MySqlClient.MySqlCommand(_configurationHandler.Configuration.Query, connection);
                this.Log.Write(Sdk.Logging.LogEntry.LogType.Debug, $"Command '{_configurationHandler.Configuration.Query}' with {command.Connection.ConnectionString} ({command.Connection.State})");
            }
            catch (Exception ex)
            {
                this.Log.Write(Sdk.Logging.LogEntry.LogType.Fatal, ex.Message);
            }
            if (command != null)
            {
                return DbCommandReader.ReadCommand(command, ds.FieldDefinitions, this.Log);
            }
            
            return null;
        }

        protected override void OnBuilt()
        {
            base.OnBuilt();
            _configurationHandler = new ConfigurationHandler<SqlImporterConfiguration>(this.Definition);
        }

    }
}
