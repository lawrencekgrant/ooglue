
using System;
using System.Data;

namespace ooglue.Access
{


	public abstract class DataAccess : ISqlSyntax
	{
		public abstract IDataReader ExecuteProcedure (IDbConnection connection, string procedureName, params IDataParameter[] parameters);
		public abstract IDataReader ExecuteSql(IDbConnection connection, string commandText);
		public abstract object ExecuteScalar(IDbConnection connection, string commandText);
		
		#region ISqlSyntax implementation
		public abstract string SelectTemplate {get;}
		public abstract string UpdateTemplate {get;}
		public abstract string DeleteTemplate {get;}
		public abstract string InsertTemplate {get;}
		public abstract string CreateTableTemplate {get;}
		#endregion
		
		public abstract string ConnectionString { get; }
		public abstract IDbConnection NewConnection { get; }
		public abstract string ParameterPrefix { get; }
	}
}
