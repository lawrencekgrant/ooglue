
using System;
using System.Configuration;
using System.Data;
using Mono.Data.SqliteClient;

namespace ooglue.Access
{
	public class SqliteAccess : DataAccess
	{
		public override string ConnectionString {
			get 
			{
				return ConfigurationManager.ConnectionStrings["SqliteConnectionString"].ConnectionString;
			}
		}
		
		public override IDbConnection NewConnection 
		{
			get 
			{
				new SqliteConnection(ConnectionString);
			}
		}
		
		public override string ParameterPrefix 
		{
			get 
			{
				return string.Empty;
			}
		}
		
		
		
		public override IDataReader ExecuteProcedure (System.Data.IDbConnection connection, string procedureName, params IDataParameter[] parameters)
		{
			IDbCommand command = connection.CreateCommand();
			command.CommandText = procedureName;
			command.CommandType = CommandType.StoredProcedure;
			command.Parameters = parameters;
			return command.ExecuteReader();
		}
		
		
		
		public SqliteAccess ()
		{
		}
		public override IDataReader ExecuteSql (System.Data.IDbConnection connection, string commandText)
		{
			IDbCommand command = connection.CreateCommand();
			command.CommandText = commandText;
			command.CommandType = CommandType.Text;
			return command.ExecuteReader();
		}
		
		#region ISqlSyntax implementation
		public override string SelectTemplate 
		{
			get 
			{
				return "select {0} from {1} {2}";
			}
		}
		
		
		public override string UpdateTemplate { 
			get 
			{ 
				return "update {0} set {1} where {2} = {3}"; 
			}
		}
		
		
		public override string DeleteTemplate 
		{
			get 
			{
				return "delete from {0} where {1} = '{2}'";
			}
		}
		
		
		public override string InsertTemplate 
		{
			get 
			{
				return "insert into {0} ({1}) values ({2})";
			}
		}
		
		#endregion
		
		
	}
}
