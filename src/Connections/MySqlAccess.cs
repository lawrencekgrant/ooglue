
using System;
using System.Configuration;
using System.Data;


using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ooglue.Connections
{
	public class MySqlAccess : DataAccess
	{

		public MySqlAccess ()
		{
		}
		
		public override IDataReader ExecuteReader(IDbConnection connection, string storedProcedureName, params IDataParameter [] mySqlParams)
		{
			MySqlDataReader returnReader;
			MySqlCommand command = new MySqlCommand(storedProcedureName, (MySqlConnection)connection);
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.AddRange(mySqlParams);
			returnReader = command.ExecuteReader();			
			return (IDataReader)returnReader;
		}
		
		public override IDbConnection NewConnection
		{
			get
			{
				return (IDbConnection)new MySqlConnection(ConnectionString);
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
