
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
	public class DataExchange
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(DataExchange));
		
		public DataExchange ()
		{
		}
		
		public static T GetObjectFromDataReader<T>(IDataReader reader) where T : new()
		{
			List<T> returnItemList = GetFromDataReader<T>(reader, 1);
			
			if(returnItemList != null)
				return returnItemList[0];
			throw new Exception("There were no items created from the source IDataReader.");
		}
		
		public static List<T> GetFromDataReader<T>(IDataReader reader) where T : new()
		{
			return GetFromDataReader<T>(reader, int.MaxValue);
		}
		
		public static List<T> GetFromDataReader<T>(IDataReader reader, int count) where T : new()
		{
			List<T> returnList = new List<T>();
			Type tType = typeof(T);
			int counter = 0;
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
			
			while(reader.Read() && count != 0 && counter++ < count)
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
								//log.InfoFormat("Mapping {0} to {1}", reader.GetValue(i).ToString(), info.Name);
								if(info.PropertyType == typeof(bool))
									info.SetValue(t, reader.GetBoolean(i), null);
								else
									info.SetValue(t, reader.GetValue(i), null);
							}
							catch(Exception ex)
							{
								log.Error(ex.ToString());							}
							}
					}
					catch(Exception ex)
					{
						log.ErrorFormat("Could not map field {0}: {1}", i, ex.ToString());
						throw;
					}
				}
				returnList.Add(t);
			}
			Debug.WriteLine(returnList.ToString());
			return returnList;
		}
		
		public static IDbCommand GetCommandFromObject<T>(DataAccess access, T paramsObject)
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
		
		public static IDbCommand GetDynamicInsertFromObject<T>(DataAccess access, T paramsObject)
		{
			const string templateString = "insert into {0} ({1}) values ({2})";
			const string valueTemplateString = "'{0}'";
			string dataTableName = string.Empty;
			List<string> fields = new List<string>();
			List<string> values = new List<string>();
			List<TableAttribute> dataTableAttributes =
				new List<TableAttribute>((TableAttribute[])typeof(T).GetCustomAttributes(typeof(TableAttribute), true));
			
			IDbConnection connection = access.NewConnection;
			IDbCommand command = connection.CreateCommand();
			
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
			
			command.CommandText = string.Format(
			                                    templateString, 
			                                    dataTableName, 
			                                    string.Join(",", fields.ToArray()), 
			                                    string.Join(",", values.ToArray()));
			
			command.CommandType = CommandType.Text;
			
			return command;
		}
		
		
		public static IDbCommand GetDynamicUpdateFromObject<T>(DataAccess access, T paramsObject)
		{
			const string templateString = "update {0} set {1} where {2} = {3}";
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
			
			IDbCommand command = access.NewConnection.CreateCommand ();
			command.CommandText = string.Format(
			                                    templateString, 
			                                    dataTableName, 
			                                    string.Join(",", updateValues.ToArray()),
			                                    primaryKey.Key,
			                                    primaryKey.Value);
			
			command.CommandType = CommandType.Text;
			
			return command;
		}
		
		public static IDbCommand GetDynamicDeleteFromObject<T>(DataAccess access, T paramsObject)
		{
			const string commandTextTemplate = "delete from {0} where {1} = '{2}'";
			
			string tableName;
			KeyValuePair<string,string> primaryKey;
			IDbConnection connection = access.NewConnection;
			IDbCommand command = connection.CreateCommand();
			
			tableName = getTableNameFromObject<T>(paramsObject);
			primaryKey = getPrimaryKeyFromObject<T>(paramsObject);
			
			command.CommandText = string.Format(
			                                    commandTextTemplate,
			                                    tableName,
			                                    primaryKey.Key,
			                                    primaryKey.Value);
			command.CommandType = CommandType.Text;
			return command;
		}
		
		public static IDbCommand GetDynamicSelectFromObject<T>(DataAccess access, T paramsObject)
		{
			const string commandText = "select {0} from {1} {2}";
			string dataTableName = getTableNameFromObject<T>(paramsObject);
			IDbCommand command = access.NewConnection.CreateCommand();
			command.CommandText = string.Format(
			                                    commandText,
			                                    getFieldListFromObject<T>(paramsObject),
			                                    dataTableName,
			                                    getWhereListFromObject<T>(paramsObject));
			command.CommandType = CommandType.Text;
			
			return command;
		}
		
		internal static string getFieldListFromObject<T>(T searchObject)
		{
			StringBuilder builder = new StringBuilder();
			
			var items = from pinfo in typeof(T).GetProperties() 
				from attr in (ColumnAttribute[]) pinfo.GetCustomAttributes(typeof(ColumnAttribute), true)
				select attr.Name;
			
			return string.Join(" , ", items.ToArray());
		}
		
		internal static string getWhereListFromObject<T>(T searchObject)
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
		
		internal static KeyValuePair<string, string> getPrimaryKeyFromObject<T>(T searchObject)
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
		
		internal static string getTableNameFromObject<T>(T searchObject)
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
