using System;
using System.Configuration;
using System.Data;
using Npgsql;

namespace ooglue.Access
{
	public class PGSqlAccess : DataAccess
	{
				
		/// <summary>
		/// This method executes a routine against a database and returns data.
		/// </summary>
		/// <param name="connection">
		/// A <see cref="IDbConnection"/> with an open connection is required to execute the routine.
		/// </param>
		/// <param name="procedureName">
		/// A <see cref="System.String"/> containing the name of the routine to be run.
		/// </param>
		/// <param name="parameters">
		/// A <see cref="IDataParameter[]"/> containing a list a parameters to be used with the routine.
		/// </param>
		/// <returns>
		/// A <see cref="IDataReader"/> is returned containing the results of the executed routine.
		/// </returns>
		public override IDataReader ExecuteProcedure(IDbConnection connection, string procedureName, params IDataParameter [] parameters)
		{
			NpgsqlDataReader returnReader;
			NpgsqlCommand command = new NpgsqlCommand(procedureName, (NpgsqlConnection)connection);
			command.CommandType = System.Data.CommandType.StoredProcedure;
			command.Parameters.AddRange(parameters);
			returnReader = command.ExecuteReader();			
			return returnReader;
		}
		
		/// <summary>
		/// This method executes a SQL statement against a connection.
		/// </summary>
		/// <param name="connection">
		/// A <see cref="IDbConnection"/> to execute the statement against.
		/// </param>
		/// <param name="commandText">
		/// A <see cref="System.String"/> containing the statement to execute.
		/// </param>
		/// <returns>
		/// A <see cref="IDataReader"/> is returned containing the results of the executed statement.
		/// </returns>
		public override IDataReader ExecuteSql (IDbConnection connection, string commandText)
		{
			if(connection == null)
				throw new NullReferenceException("Cannot Execute SQL against a null connection.");
			
			NpgsqlCommand command = (NpgsqlCommand)connection.CreateCommand ();
			command.CommandText = commandText;
			command.CommandType = CommandType.Text;
			return command.ExecuteReader ();
		}
				
		public override object ExecuteScalar(IDbConnection connection, string commandText)
		{
			if(connection == null)
				throw new NullReferenceException("Cannot execute SQL against a null connection.");
			
			if(connection.State != ConnectionState.Broken)
				throw new ooglueException(new InvalidOperationException("Broken connection. Aborting."));
			try
			{
				if(connection.State == ConnectionState.Closed)
					connection.Open();
				
				NpgsqlCommand command = (NpgsqlCommand)connection.CreateCommand();
				command.CommandText = commandText;
				command.CommandType = CommandType.Text;
				return command.ExecuteScalar();
			}
			finally
			{
				if(connection.State == ConnectionState.Open)
					connection.Close();
			}
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
		
		public override string CreateTableTemplate 
		{
			get 
			{
				return "create table {0}";
			}
		}

		#endregion
		/// <summary>
		/// Returns a new MySqlConnection object with the default connection string.
		/// </summary>
		public override IDbConnection NewConnection
		{
			get
			{
				return (IDbConnection) new NpgsqlConnection(ConnectionString);
			}
		}
		
		/// <summary>
		/// The default connection string for the data access object.
		/// </summary>
		public override string ConnectionString
		{
			get
			{
				return ConfigurationManager.AppSettings["pgsqlConnectionString"];
			}
		}
		
		/// <summary>
		/// The prefix to use when specifying a parameter name.
		/// </summary>
		public override string ParameterPrefix
		{
			get
			{
				return "@";
			}
		}
	}
}