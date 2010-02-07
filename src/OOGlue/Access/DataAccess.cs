
using System;
using System.Data;

namespace ooglue.Access
{


	public abstract class DataAccess
	{
		public abstract IDataReader ExecuteProcedure (IDbConnection connection, string procedureName, params IDataParameter[] parameters);
		public abstract IDataReader ExecuteSql(IDbConnection connection, string commandText);
		public abstract string ConnectionString { get; }
		public abstract IDbConnection NewConnection { get; }
		public abstract string ParameterPrefix { get; set; }
	}
}
