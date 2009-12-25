
using System;
using System.Data;

using ooglue.Access;

namespace ooglue
{


	public abstract class DataAccess
	{
		public abstract IDataReader ExecuteProcedure (IDbConnection connection, string storeProcedureName, params IDataParameter[] parameters);
		public abstract IDataReader ExecuteSql(IDbConnection connection, string sql);
		public abstract string ConnectionString { get; }
		public abstract IDbConnection NewConnection { get; }
		public abstract string ParameterPrefix { get; set; }
	}
}
