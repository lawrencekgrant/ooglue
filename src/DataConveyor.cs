
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
		
		public static void UpdateObject<T>(DataAccess access, T objectToUpdate)
		{
			IDbCommand command = null;
			
			try
			{
				command = DataExchange.GetDynamicUpdateFromObject<T>(access, objectToUpdate);
				command.Connection.Open();
				command.ExecuteNonQuery();
			}
			catch { throw; }
			finally
			{
				if(command != null && command.Connection != null)
				{
					command.Connection.Close();
					command.Connection.Dispose();
					command.Dispose();
				}
			}
		}
		
		public static void InsertObject<T>(DataAccess access, T objectToInsert)
		{
			IDbCommand command = null;
			
			try
			{
				command = DataExchange.GetDynamicInsertFromObject(access, objectToInsert);
				command.Connection.Open();
				command.ExecuteNonQuery();
			}
			catch { throw; }
			finally
			{
				if(command != null && command.Connection != null)
				{
					command.Connection.Close();
					command.Connection.Dispose();
					command.Dispose();
				}
			}
		}
		
		public static void DeleteObject<T>(DataAccess access, T objectToDelete)
		{
			
		}
		
		public static List<T> FetchObjectListByProcedure<T>(DataAccess access, string storedProcedureName, params IDataParameter [] parameters) where T: new()
		{
			IDataReader reader;
			IDbConnection connection = null;
			List<T> returnList;
			try
			{
				connection = access.NewConnection;
				connection.Open();
				using(reader = access.ExecuteProcedure(connection, storedProcedureName, parameters))
				{
					returnList = DataExchange.GetFromDataReader<T>(reader);
				}
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
			List<T> returnList;
			try
			{
				connection = access.NewConnection;
				connection.Open();
				using(reader = access.ExecuteSql(connection, commandText))
				{
					returnList = DataExchange.GetFromDataReader<T>(reader);
				}
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
