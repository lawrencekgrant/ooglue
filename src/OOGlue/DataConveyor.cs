
using System;
using System.Collections.Generic;
using System.Data;

using ooglue.Access;
using log4net;

namespace ooglue
{


	public class DataConveyor
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DataConveyor));
		
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
				log.DebugFormat("Connection String for insert: {0}", access.ConnectionString);
				log.DebugFormat("Object is {0}", objectToInsert.ToString());
				command = DataExchange.GetDynamicInsertFromObject(access, objectToInsert);
				log.DebugFormat("SQL: {0}", command.CommandText);
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
			IDbCommand command = null;
			
			try
			{
				log.DebugFormat("Connection String for insert: {0}", access.ConnectionString);
				log.DebugFormat("Object is {0}", objectToDelete.ToString());
				command = DataExchange.GetDynamicDeleteFromObject(access, objectToDelete);
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
