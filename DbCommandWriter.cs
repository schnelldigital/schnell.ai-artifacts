using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

public class DbCommandWriter
{
    public static async Task<int> WriteCommand(DbCommand command, List<DbParameter> parameters, IEnumerable<Schnell.Ai.Sdk.Definitions.FieldDefinition> fieldDefinitions)
    {
        command.Parameters.Clear();
        parameters.Where(p => fieldDefinitions.Any(fd => String.Equals(fd.Name, p.ParameterName))).ToList().ForEach(p => command.Parameters.Add(p));
        return command.ExecuteNonQuery();
    }
}
