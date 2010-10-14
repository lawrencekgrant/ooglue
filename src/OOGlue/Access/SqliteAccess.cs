
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;

namespace ooglue.Access
{
	/// <summary>
	/// This class is a concrete implementation of the <see cref="ooglue.Access.DataAccess"/> class. 
	/// This class is specific to SQLite.
	/// </summary>
	/// <remarks>
	/// One thing that I really need to think about is seeing if there is a way that I can make this .NET compatible easliy, without
	/// including Mono.Data.SQLite. It is compatible with that assembly referenced, however, .NET (obviously) has no Mono.Data.SQLite.
	/// </remarks>
	public class SQLiteAccess : DataAccess
	{
		/// <summary>
		/// Returns the default connection string for a SQLite database
		/// </summary>
		public override string ConnectionString {
			get 
			{
				return DataAccess.GetConnectionString();
			}
		}
		
		/// <summary>
		/// A default instantiated <see cref="System.Data.IDbConnection"/> for a SQLite database.
		/// </summary>
		public override IDbConnection NewConnection 
		{
			get 
			{
				return new SQLiteConnection(ConnectionString);
			}
		}
		
		/// <summary>
		/// The default prefix for a parameter using s SQLite database.
		/// </summary>
		public override string ParameterPrefix 
		{
			get 
			{
				return string.Empty;
			}
		}
		
		
		/// <summary>
		/// Executes a stored procedure against a SQLite database.
		/// </summary>
		/// <param name="connection">
		/// A <see cref="System.Data.IDbConnection"/> used to connect to the database and execute SQL statements.
		/// </param>
		/// <param name="procedureName">
		/// A <see cref="System.String"/> that defines the name of the stored procedure to call.
		/// </param>
		/// <param name="parameters">
		/// A list of <see cref="IDataParameter[]"/> that defines the parameters to call the stored procedure with.
		/// </param>
		/// <returns>
		/// A <see cref="IDataReader"/> containing the data returned from the stored procedure.
		/// </returns>
		public override IDataReader ExecuteProcedure (System.Data.IDbConnection connection, 
		                                              string procedureName, 
		                                              params IDataParameter[] parameters)
		{
			IDbCommand command = connection.CreateCommand();
			command.CommandText = procedureName;
			command.CommandType = CommandType.StoredProcedure;
			new List<IDataParameter>(parameters).ForEach(param => 
			{
				command.Parameters.Add(param);	
			});
			return command.ExecuteReader();
		}
		
		
		/// <summary>
		/// Creates a new SQLiteAccess instance.
		/// </summary>
		public SQLiteAccess ()
		{
		}
		
		/// <summary>
		/// Executese a custom sql string against the specified data connection.
		/// </summary>
		/// <param name="connection">
		/// A <see cref="System.Data.IDbConnection"/>
		/// </param>
		/// <param name="commandText">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="IDataReader"/>
		/// </returns>
		public override IDataReader ExecuteSql (System.Data.IDbConnection connection, string commandText)
		{
			IDbCommand command = connection.CreateCommand();
			command.CommandText = commandText;
			command.CommandType = CommandType.Text;
			return command.ExecuteReader();
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
				
				SQLiteCommand command = (SQLiteCommand)connection.CreateCommand();
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
		
		
	}
}
