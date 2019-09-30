using Schnell.Ai.Sdk.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

public class DbCommandReader
{
	public static IEnumerable<IDictionary<string, object>> ReadCommand(DbCommand command, IEnumerable<Schnell.Ai.Sdk.Definitions.FieldDefinition> fieldDefinitions, Logger log, bool closeConnection = true)
	{
        var recordsReaded = 0;
        if (command.Connection?.State != System.Data.ConnectionState.Open)
            command.Connection.Open();
        DbDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            var record = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                var fieldName = reader.GetName(i);
                var fieldDefinition = fieldDefinitions.FirstOrDefault(f => String.Equals(f.Name, fieldName, StringComparison.InvariantCultureIgnoreCase));
                
                if(fieldDefinition != null)
                {
                    switch(fieldDefinition.ValueType)
                    {
                        case Schnell.Ai.Sdk.Definitions.FieldDefinition.ValueTypeEnum.Boolean: record.Add(fieldDefinition.Name, reader.GetBoolean(i)); break;
                        case Schnell.Ai.Sdk.Definitions.FieldDefinition.ValueTypeEnum.Float: record.Add(fieldDefinition.Name, reader.GetFloat(i)); break;
                        case Schnell.Ai.Sdk.Definitions.FieldDefinition.ValueTypeEnum.Integer: record.Add(fieldDefinition.Name, reader.GetInt32(i)); break;
                        case Schnell.Ai.Sdk.Definitions.FieldDefinition.ValueTypeEnum.String: record.Add(fieldDefinition.Name, reader.GetString(i)); break;
                        case Schnell.Ai.Sdk.Definitions.FieldDefinition.ValueTypeEnum.Unspecified: record.Add(fieldDefinition.Name, reader.GetValue(i)); break;
                    }                    
                }                
            }

            var orderedRecord = new Dictionary<string, object>();
            fieldDefinitions.ToList().ForEach(fd =>
            {
                if(record.ContainsKey(fd.Name))
                    orderedRecord.Add(fd.Name, record[fd.Name]);
                else
                    orderedRecord.Add(fd.Name, null);
            });
            recordsReaded++;
            log.Progress(currentValue: recordsReaded);
            yield return orderedRecord;
        }
        if (closeConnection)
            command.Connection.Close();
        log.ProgressCompleted();
    }
}
