
using System;
using System.Data;

using ooglue.Connections;

namespace ooglue
{


	public abstract class DataAccess
	{
		public abstract IDataReader ExecuteReader(IDbConnection connection, string storeProcedureName, params IDataParameter [] parameters);
		public abstract string ConnectionString {get;}
		public abstract IDbConnection NewConnection{get;}
	}
}
