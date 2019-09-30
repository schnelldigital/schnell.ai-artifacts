using Newtonsoft.Json;
using Schnell.Ai.Sdk.Configuration;
using Schnell.Ai.Sdk.DataSets;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Schnell.Ai.Artifacts.Sql
{
    /// <summary>
    /// Configuration for SQL importer
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
      

    public class SqlImporter : Sdk.DataSets.DataSetImporter
    {
        private ConfigurationHandler<SqlImporterConfiguration> _configurationHandler;
        public override IConfigurationHandler ConfigurationHandler => _configurationHandler;

        public override async Task<IEnumerable<IDictionary<string, object>>> Import(DataSet ds)
        {

            using (SqlConnection connection = new SqlConnection(_configurationHandler.Configuration.ConnectionString))
            {                
                DbCommand command = null;
                try
                {
                    command = new SqlCommand(_configurationHandler.Configuration.Query, connection);
                    command.Connection.Open();                    
                }catch(Exception ex)
                {
                    this.Log.Write(Sdk.Logging.LogEntry.LogType.Fatal, ex.Message);
                }
                if (command != null)
                {
                    return DbCommandReader.ReadCommand(command, ds.FieldDefinitions, this.Log);
                }
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
