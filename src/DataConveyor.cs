
using System;
using System.Collections.Generic;
using System.Data;

using ooglue.Access;

namespace ooglue
{


	public class DataConveyor
	{
		public DataConveyor ()
		{
		}
		
		public static void SaveObject<T>(DataAccess access, T objectToSave)
		{
			IDbCommand command = null;
			
			try
			{
				command = DataExchange.GetDynamicInsertFromObject(access, objectToSave);
				command.Connection.Open();
				command.ExecuteNonQuery();
			}
			catch { throw; }
			finally
			{
				if(command != null && command.Connection != null)
					command.Connection.Close();
			}
		}
		
		public static List<T> FetchObjectListByProcedure<T>(DataAccess access, string storedProcedureName, params IDataParameter [] parameters) where T: new()
		{
			IDataReader reader;
			IDbConnection connection = null;
			
			try
			{
				connection = access.NewConnection;
				connection.Open();
				reader = access.ExecuteProcedure(connection, storedProcedureName, parameters);
				List<T> returnList = DataExchange.GetFromDataReader<T>(reader);
				return returnList;
			}
			catch
			{
				throw;
			}
			finally
			{
				if(connection != null)
					connection.Close();
			}
		}
	
		public static List<T> FetchObjectListBySql<T>(DataAccess access, string commandText) where T: new()
		{
			IDataReader reader;
			IDbConnection connection = null;
			try
			{
				connection = access.NewConnection;
				connection.Open();
				reader = access.ExecuteSql(connection, commandText);
				List<T> returnList = DataExchange.GetFromDataReader<T>(reader);
				return returnList;
				
			}
			catch(Exception ex)
			{
				throw;
			}
			finally
			{
				if(connection != null)
					connection.Close();
			}
		}
	
	}
}
