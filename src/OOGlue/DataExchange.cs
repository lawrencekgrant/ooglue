using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using ooglue.Access;
using log4net;

namespace ooglue
{
	/// <summary>
	/// The data exchange is responsible for converting data between formats, i.e. mapping from a data source to a list of objects, and vice
	/// versa. 
	/// </summary>
	public class DataExchange
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DataExchange));
		protected DataAccess access;
		
		/// <summary>
		/// Creates a new instance of a DataExchange
		/// </summary>
		/// <param name="access">
		/// A <see cref="DataAccess"/> that determines the data access method for the exchange to be created.
		/// </param>
		public DataExchange (DataAccess dataAccess)
		{
			this.access = dataAccess;
		}
		
		
		/// <summary>
		/// Retrieves a single instantiated and populated object from a data reader.
		/// </summary>
		/// <param name="reader">
		/// A <see cref="IDataReader"/> containing the data to be mapped to the object.
		/// </param>
		/// <returns>
		/// A <see cref="T"/> representing the type to be mapped and returned.
		/// </returns>
		/// <remarks>
		/// The T parameter must be able to instantiated with a default, blank constructor. 
		///</remarks>
		public T GetObjectFromDataReader<T>(IDataReader reader) where T : new()
		{
			List<T> returnItemList = GetFromDataReader<T>(reader);
			
			if(returnItemList != null && returnItemList.Count > 0)
				return returnItemList[0];
			
			return default(T);
		}
		
		public List<T> GetFromDataReader<T>(IDataReader reader) where T : new()
		{
			List<T> returnList = new List<T>();
			Type tType = typeof(T);
			Dictionary<string, PropertyInfo> propertyMap = new Dictionary<string, PropertyInfo>();
			List<PropertyInfo> propertyInfos = new List<PropertyInfo>(tType.GetProperties());
			foreach(PropertyInfo pInfo in propertyInfos)
			{
				List<Attribute> attribs = new List<Attribute>((Attribute[])pInfo.GetCustomAttributes(typeof(ColumnAttribute), true));
				foreach(ColumnAttribute dcAttr in attribs)
				{
					propertyMap.Add(dcAttr.Name, pInfo);
				}
			}
			
			while(reader.Read())
			{
				T t = new T();
				string fieldName = string.Empty;
				for(int i = 0; i < reader.FieldCount; i++)
				{
					try
					{
						if(reader.IsDBNull(i))
							continue;
						
						fieldName = reader.GetName(i);
						if(propertyMap.ContainsKey(fieldName))
						{
							try
							{
								PropertyInfo info = propertyMap[fieldName];
								log.DebugFormat("Mapping {0} to {1}", reader.GetValue(i).ToString(), info.Name);
								if(info.PropertyType == typeof(bool))
									info.SetValue(t, reader.GetBoolean(i), null);
								else
									info.SetValue(t, reader.GetValue(i), null);
							}
							catch(TargetException ex)
							{
								log.ErrorFormat("ooglue.DataExchange() - There was an error setting a property value on {0} from an object of the type {1}.", fieldName, typeof(T).ToString(),ex.ToString());							}
							}
					}
					catch(Exception ex)
					{
						log.ErrorFormat("ooglue.DataExchange() - Could not map field {0}: {1}", i, ex.ToString());
						throw;
					}
				}
				returnList.Add(t);
			}
			return returnList;
		}
		
		public IDbCommand GetCommandFromObject<T>(T paramsObject)
		{
			IDbConnection connection = access.NewConnection;
			IDbCommand command = connection.CreateCommand();
			
			foreach(PropertyInfo propertyInfo in typeof(T).GetProperties())
			{				
				List<ColumnAttribute> columnAttributes = 
					new List<ColumnAttribute>((ColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true));
				if(columnAttributes.Count > 0)
				{
					IDbDataParameter parameter = command.CreateParameter();
					parameter.ParameterName = access.ParameterPrefix + propertyInfo.Name;
					parameter.Value = propertyInfo.GetValue(paramsObject, null);
					command.Parameters.Add(parameter);
				}
			}
			
			return command;
		}
		
		public IDbCommand GetCommandFromObject<T>(T paramsObject, string commandText, CommandType commandType)
		{
			IDbCommand returnCommand = GetCommandFromObject<T>(paramsObject);
			returnCommand.CommandText = commandText;
			returnCommand.CommandType = commandType;
			return returnCommand;
		}
		
		public string GetDynamicInsertStringFromObject<T>(T paramsObject)
		{
			string templateString = access.InsertTemplate;
            const string valueTemplateString = "'{0}'";
            string dataTableName = string.Empty;
			
			List<string> fields = new List<string>();
			List<string> values = new List<string>();
			
			List<TableAttribute> dataTableAttributes =
				new List<TableAttribute>((TableAttribute[])typeof(T).GetCustomAttributes(typeof(TableAttribute), true));
			
			if(dataTableAttributes.Count > 0)
			{
				dataTableName = dataTableAttributes[0].Name;
			}
			foreach(PropertyInfo propertyInfo in typeof(T).GetProperties())
			{	
				List<ColumnAttribute> columnAttributes = 
					new List<ColumnAttribute>((ColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true));
				
				if(columnAttributes.Count > 0)
				{
					try
					{
					if(columnAttributes[0].IsPrimaryKey)
						continue;
					
					fields.Add(columnAttributes[0].Name);
					values.Add(string.Format(valueTemplateString,propertyInfo.GetValue(paramsObject, null)));
					}
					catch(Exception ex)
					{
						log.Error(ex.ToString());
					}
				}
			}
			
			return string.Format(
					            templateString, 
					            dataTableName, 
					            string.Join(",", fields.ToArray()), 
					            string.Join(",", values.ToArray()));
		}
		
		
		public string GetDynamicUpdateStringFromObject<T>(T paramsObject)
		{
			string templateString = access.UpdateTemplate;
			const string valueTemplateString = "{0} = '{1}'";
			
			string dataTableName = string.Empty;
			KeyValuePair<string,string> primaryKey = new KeyValuePair<string, string>();
			List<string> updateValues = new List<string>();
			
			dataTableName = getTableNameFromObject<T>(paramsObject);
			
			foreach(PropertyInfo propertyInfo in typeof(T).GetProperties())
			{					
				List<ColumnAttribute> columnAttributes = 
					new List<ColumnAttribute>((ColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true));
				if(columnAttributes.Count > 0)
				{
					ColumnAttribute column = columnAttributes[0];
					if(column.IsPrimaryKey)
					{
						primaryKey = new KeyValuePair<string, string>(column.Name, propertyInfo.GetValue(paramsObject, null).ToString());
						continue;
					}
					else
					{
						updateValues.Add(string.Format(valueTemplateString, columnAttributes[0].Name, propertyInfo.GetValue(paramsObject, null)));
					}
				}
			}
			
			return string.Format(
								templateString, 
								dataTableName, 
								string.Join(",", updateValues.ToArray()),
								primaryKey.Key,
								primaryKey.Value);
		}
		
		public IDbCommand GetDynamicUpdateFromObject<T>(T paramsObject)
		{
			string updateString = GetDynamicUpdateStringFromObject<T>(paramsObject);
			return GetCommandFromObject<T>(paramsObject, updateString, CommandType.Text);
		}
		
		public string GetDynamicDeleteStringFromObject<T>(T paramsObject)
		{
			string commandTextTemplate = access.DeleteTemplate;
			
			string tableName;
			KeyValuePair<string,string> primaryKey;

			tableName = getTableNameFromObject<T>(paramsObject);
			primaryKey = getPrimaryKeyFromObject<T>(paramsObject);
			
			return string.Format(
                                commandTextTemplate,
                                tableName,
                                primaryKey.Key,
                                primaryKey.Value);
		}
		
		public IDbCommand GetDynamicDeleteFromObject<T>(T paramsObject)
		{
			string deleteString = GetDynamicDeleteStringFromObject<T>(paramsObject);
			Console.WriteLine("Delete string is {0}", deleteString);
			return GetCommandFromObject<T>(paramsObject, deleteString, CommandType.Text);
		}
		
		public string GetDynamicSelectStringFromObject<T>(T paramsObject)
		{
			string commandText = access.SelectTemplate;
			string dataTableName = getTableNameFromObject<T>(paramsObject);
			return string.Format(
                                commandText,
                                getFieldListFromObject<T>(paramsObject),
                                dataTableName,
                                getWhereListStringFromObject<T>(paramsObject));
		}
		
		internal string getFieldListFromObject<T>(T searchObject)
		{
			StringBuilder builder = new StringBuilder();
			
			var items = from pinfo in typeof(T).GetProperties() 
				from attr in (ColumnAttribute[]) pinfo.GetCustomAttributes(typeof(ColumnAttribute), true)
				select attr.Name;
			
			return string.Join(" , ", items.ToArray());
		}
		
		internal string getWhereListStringFromObject<T>(T searchObject)
		{
			StringBuilder builder = new StringBuilder();
			
			var items = from pinfo in typeof(T).GetProperties()
				from attr in (ColumnAttribute[]) pinfo.GetCustomAttributes(typeof(ColumnAttribute), true)
				where (    pinfo.GetValue(searchObject, null) != null && 
					       pinfo.GetValue(searchObject, null).ToString() != Activator.CreateInstance(pinfo.PropertyType).ToString()
					       )
				select string.Format("{0} = '{1}'", attr.Name, pinfo.GetValue(searchObject, null).ToString());
			
			
			if(items.Count() > 0)
				return string.Format("where {0}", string.Join(" and ", items.ToArray()));
			else
				return string.Empty;
		}
		
		internal KeyValuePair<string, string> getPrimaryKeyFromObject<T>(T searchObject)
		{
			KeyValuePair<string,string> primaryKey = new KeyValuePair<string, string>();
			
			foreach(PropertyInfo propertyInfo in typeof(T).GetProperties())
			{	
				List<ColumnAttribute> columnAttributes = 
					new List<ColumnAttribute>((ColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true));
				
				if(columnAttributes.Count > 0)
				{
					ColumnAttribute column = columnAttributes[0];
					if(column.IsPrimaryKey)
					{
						primaryKey = new KeyValuePair<string, string>(column.Name, propertyInfo.GetValue(searchObject, null).ToString());
						break;
					}
				}
			}
			
			return primaryKey;
		}
		
		internal string getTableNameFromObject<T>(T searchObject)
		{
			string dataTableName = string.Empty;
			
			List<TableAttribute> dataTables =
				new List<TableAttribute>((TableAttribute[])typeof(T).GetCustomAttributes(typeof(TableAttribute), true));
			
			if(dataTables.Count > 0)
			{
				dataTableName = dataTables[0].Name;
			}
			return dataTableName;
		}
		
		
		/*
		private static Dictionary<string, object> GetInputParametersFromInfo(ParameterInfo [] parameters)
		{
			Dictionary<string, object> returnParams = new Dictionary<string, object>();
				
			foreach(ParameterInfo parameterInfo in parameters)
			{
				List<InAttribute> ins = new List<InAttribute>((InAttribute[])parameterInfo.GetCustomAttributes(typeof(InAttribute), true));
				
				foreach(InAttribute ina in ins)
				{
					returnParams.Add(ina.Name, parameterInfo.
				}
			}
			return returnParams;
		}
		*/
	}
}
