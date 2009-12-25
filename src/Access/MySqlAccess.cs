
using System;
using System.Configuration;
using System.Data;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ooglue.Access
{
	public class MySqlAccess : DataAccess
	{
		public override string ParameterPrefix { get; set; }

		public MySqlAccess ()
		{
		}
		
		public override IDataReader ExecuteProcedure(IDbConnection connection, string procedureName, params IDataParameter [] parameters)
		{
			MySqlDataReader returnReader;
			MySqlCommand command = new MySqlCommand(procedureName, (MySqlConnection)connection);
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.AddRange(parameters);
			returnReader = command.ExecuteReader();			
			return returnReader;
		}
		
		public override IDataReader ExecuteSql (IDbConnection connection, string commandText)
		{
			MySqlCommand command = (MySqlCommand)connection.CreateCommand ();
			command.CommandText = commandText;
			command.CommandType = CommandType.Text;
			return command.ExecuteReader ();
		}
		
		public override IDbConnection NewConnection
		{
			get
			{
				return (IDbConnection) new MySqlConnection(ConnectionString);
			}
		}
		
		public override string ConnectionString
		{
			get
			{
				return ConfigurationManager.AppSettings["mysqlConnectionString"];
			}
		}
	}
}
