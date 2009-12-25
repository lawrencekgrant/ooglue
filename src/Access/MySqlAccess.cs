
using System;
using System.Configuration;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ooglue.Connections
{
	public class MySqlAccess
	{

		public MySqlAccess ()
		{
		}
		
		public MySqlDataReader ExecuteReader(MySqlConnection connection, string storedProcedureName, params MySqlParameter [] mySqlParams)
		{
			MySqlDataReader returnReader;
			MySqlCommand command = new MySqlCommand(storedProcedureName, connection);
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.AddRange(mySqlParams);
			returnReader = command.ExecuteReader();			
			return returnReader;
		}
		
		public static MySqlConnection NewConnection
		{
			get
			{
				return new MySqlConnection(ConnectionString);
			}
		}
		
		public static string ConnectionString
		{
			get
			{
				return ConfigurationManager.AppSettings["mysqlConnectionString"];
			}
		}
	}
}
