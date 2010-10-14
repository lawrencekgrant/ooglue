
using System;
using System.Collections;
using System.Configuration;
using System.Data;

using ooglue.Configuration;

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
		
		public static DataAccess GetConfiguredDataAccess()
		{
			ConfigurationManager.RefreshSection("ooglueConfigurationSection");
			OOGlueConfigurationSection ooglueConfigSection = (OOGlueConfigurationSection)System.Configuration.ConfigurationManager.GetSection("ooglueConfigurationSection");
			Type dataAccessType = Type.GetType(ooglueConfigSection.DataAccessTypeName.ToString());
			DataAccess returnAccess = (DataAccess)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(dataAccessType);
			return returnAccess;	
		}
		
		public static string GetConnectionString()
		{
			ConfigurationManager.RefreshSection("ooglueConfigurationSection");
			OOGlueConfigurationSection ooglueConfigSection = (OOGlueConfigurationSection)System.Configuration.ConfigurationManager.GetSection("ooglueConfigurationSection");
			return ooglueConfigSection.ConnectionString.ToString();
		}
	}
}
