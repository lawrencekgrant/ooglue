
using System;
using System.Collections.Generic;
using System.Data;

using ooglue.Access;
using log4net;

namespace ooglue
{
	/// <summary>
	/// The DataConveyor is responsible for handling CRUD operations on objects.
	/// </summary>
	public class DataConveyor
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(DataConveyor));
		private DataExchange dataExchange;
		private DataAccess access;
		
		/// <summary>
		/// Constructs a new instance of a DataConveyor
		/// </summary>
		public DataConveyor (DataAccess dataAccess)
		{
			this.access = dataAccess;
			dataExchange = new DataExchange(access);
		}
		
		/// <summary>
		/// Updates an object to an access object.
		/// </summary>
		/// <param name="access">
		/// A <see cref="DataAccess"/> containing the information needed to reach the objects storage.
		/// </param>
		/// <param name="objectToUpdate">
		/// A <see cref="T"/> which will be stored as an update.
		/// </param>
		public void UpdateObject<T>(T objectToUpdate)
		{
			if(objectToUpdate == null)
				throw new ooglueException(new NullReferenceException("You cannot update a null object."));
			
			IDbCommand command = null;
				
			try
			{
				command = dataExchange.GetDynamicUpdateFromObject<T>(objectToUpdate);
				_log.DebugFormat("command text for update: {0}", command.CommandText);
				command.Connection = access.NewConnection;
				command.Connection.Open();
				command.ExecuteNonQuery();
			}
			catch(Exception ex)
			{
				_log.Error("DataConveyor.UpdateObject() - Error executing update on dynamic insert.", ex);
				throw; 
			}
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
		
		/// <summary>
		/// Inserts an object to the specified data storage.
		/// </summary>
		/// <param name="access">
		/// A <see cref="DataAccess"/> containing the information needed to reach the objects storage.
		/// </param>
		/// <param name="objectToInsert">
		/// A <see cref="T"/> which will be stored as a new object.
		/// </param>
		public void InsertObject<T>(T objectToInsert)
		{
			if(objectToInsert == null)
				throw new ooglueException(new NullReferenceException("You cannot update a null object."));
			
			IDbCommand command = null;
			
			try
			{
				_log.DebugFormat("Connection String for insert: {0}", access.ConnectionString);
				_log.DebugFormat("Object is {0}", objectToInsert.ToString());
				Console.WriteLine("Connection String for insert: {0}", access.ConnectionString);
				Console.WriteLine("Object is {0}", objectToInsert.ToString());
				IDbConnection connection = access.NewConnection;
				command = connection.CreateCommand();
				command.CommandText = dataExchange.GetDynamicInsertStringFromObject(objectToInsert);
				_log.DebugFormat("SQL: {0}", command.CommandText);
				Console.WriteLine("SQL: {0}", command.CommandText);
				command.Connection.Open();
				int result = command.ExecuteNonQuery();
				Console.WriteLine("executed. result is {0}", result);
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
		
		/// <summary>
		/// Removes an object from data storage.
		/// </summary>
		/// <param name="access">
		/// A <see cref="DataAccess"/> containing information needed to reach the object's storage.
		/// </param>
		/// <param name="objectToDelete">
		/// A <see cref="T"/> which will be deleted.
		/// </param>
		public void DeleteObject<T>(T objectToDelete)
		{
			if(objectToDelete == null)
				throw new ooglueException(new NullReferenceException("You cannot delete a null object."));
			
			IDbCommand command = null;
			
			try
			{
				_log.DebugFormat("Connection String for delete: {0}", access.ConnectionString);
				Console.WriteLine("Deleting object of type {0}", objectToDelete.ToString());
				command = dataExchange.GetDynamicDeleteFromObject<T>(objectToDelete);
				
				command.Connection = access.NewConnection;
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
		
		/// <summary>
		/// Retrieves a list of objects of the specified type.
		/// </summary>
		/// <param name="access">
		/// A <see cref="DataAccess"/> containing information needed to reach the object's storage.
		/// </param>
		/// <param name="storedProcedureName">
		/// A <see cref="System.String"/> containing the name of the stored procedure to be called.
		/// </param>
		/// <param name="parameters">
		/// A <see cref="IDataParameter[]"/> containing the list of parameters to be used with the stored procedure.
		/// </param>
		/// <returns>
		/// A <see cref="List<T>"/> is returned. The members of the returned list are of the type specified on the method call.
		/// </returns>
		public List<T> FetchObjectListByProcedure<T>(string storedProcedureName, params IDataParameter [] parameters) where T: new()
		{
			IDataReader reader;
			IDbConnection connection = null;
			List<T> returnList = new List<T>();
			try
			{
				connection = access.NewConnection;
				connection.Open();
				using(reader = access.ExecuteProcedure(connection, storedProcedureName, parameters))
				{
					returnList = dataExchange.GetFromDataReader<T>(reader);
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
	
		
		/// <summary>
		/// Retrieves a list of objects of the specified type using a SQL statement.
		/// </summary>
		/// <param name="access">
		/// A <see cref="DataAccess"/> containing information needed to reach the object's storage.
		/// </param>
		/// <param name="storedProcedureName">
		/// A <see cref="System.String"/> containing the statement to be executed.
		/// </param>
		/// <param name="parameters">
		/// A <see cref="IDataParameter[]"/> containing the list of parameters to be used with the stored procedure.
		/// </param>
		/// <returns>
		/// A <see cref="List<T>"/> is returned. The members of the returned list are of the type specified on the method call.
		/// </returns>
		public List<T> FetchObjectListBySql<T>(string commandText) where T: new()
		{
			IDataReader reader;
			IDbConnection connection = null;
			List<T> returnList = new List<T>();
			try
			{
				_log.DebugFormat("Data access is {0} null.", access == null ? string.Empty : "not");
				connection = access.NewConnection;
				connection.Open();
				_log.DebugFormat("Connection to execute {0} has been opened.", commandText);
				using(reader = access.ExecuteSql(connection, commandText))
				{
					returnList = dataExchange.GetFromDataReader<T>(reader);
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
