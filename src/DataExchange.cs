
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using ooglue.Access;

namespace ooglue
{
	public class DataExchange
	{
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
						PropertyInfo info = propertyMap[fieldName];
						System.Console.WriteLine("Mapping {0} to {1})", reader.GetValue(i).ToString(), info.Name);
						info.SetValue(t, reader.GetValue(i), null);
					}
					}
					catch(Exception ex)
					{
						Console.WriteLine("Error:" + ex.ToString());
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
				List<DataColumnAttribute> columnAttributes = 
					new List<DataColumnAttribute>((ColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true));
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
			string templateString = "insert into {0} ({1}) values ({2})";
			string valueTemplateString = "'{0}'";
			string dataTableName = string.Empty;
			List<string> fields = new List<string>();
			List<string> values = new List<string>();
			List<DataTableAttribute> dataTableAttributes =
				new List<DataTableAttribute>((TableAttribute[])typeof(T).GetCustomAttributes(typeof(TableAttribute), true));
			
			IDbConnection connection = access.NewConnection;
			IDbCommand command = connection.CreateCommand();
			
			if(dataTableAttributes.Count > 0)
			{
				dataTableName = dataTableAttributes[0].Name;
			}
			foreach(PropertyInfo propertyInfo in typeof(T).GetProperties())
			{	
				List<KeyFieldAttribute> keyFields =
					new List<KeyFieldAttribute>((KeyFieldAttribute[])propertyInfo.GetCustomAttributes(typeof(KeyFieldAttribute), true));
				
				if(keyFields.Count > 0)
				{
					continue;
				}
				
				List<DataColumnAttribute> columnAttributes = 
					new List<DataColumnAttribute>((ColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true));
				if(columnAttributes.Count > 0)
				{
					fields.Add(columnAttributes[0].Name);
					values.Add(string.Format(valueTemplateString,propertyInfo.GetValue(paramsObject, null)));
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
			string templateString = "update {0} set {1} where {2} = {3}";
			string valueTemplateString = "{0} = '{1}'";
			string dataTableName = string.Empty;
			string primaryKeyId = string.Empty;
			string primaryKeyValue = string.Empty;
			List<DataTableAttribute> dataTableAttributes =
				new List<DataTableAttribute>((TableAttribute[])typeof(T).GetCustomAttributes(typeof(TableAttribute), true));
			
			List<string> updateValues = new List<string>();
			
			IDbConnection connection = access.NewConnection;
			IDbCommand command = connection.CreateCommand();
			
			if(dataTableAttributes.Count > 0)
			{
				dataTableName = dataTableAttributes[0].Name;
			}
			
			foreach(PropertyInfo propertyInfo in typeof(T).GetProperties())
			{	
				List<KeyFieldAttribute> keyFields =
					new List<KeyFieldAttribute>((KeyFieldAttribute[])propertyInfo.GetCustomAttributes(typeof(KeyFieldAttribute), true));
				
				if(keyFields.Count > 0)
				{
					primaryKeyId = keyFields[0].Name;
					primaryKeyValue = propertyInfo.GetValue(paramsObject, null).ToString();
					continue;
				}
				
				List<DataColumnAttribute> columnAttributes = 
					new List<DataColumnAttribute>((ColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), true));
				if(columnAttributes.Count > 0)
				{
					updateValues.Add(string.Format(valueTemplateString, columnAttributes[0].Name, propertyInfo.GetValue(paramsObject, null)));
				}
			}
			
			command.CommandText = string.Format(
			                                    templateString, 
			                                    dataTableName, 
			                                    string.Join(",", updateValues.ToArray()),
			                                    primaryKeyId,
			                                    primaryKeyValue);
			
			command.CommandType = CommandType.Text;
			
			return command;
		}
		
		public static IDbCommand GetDynamicDeleteFromObject<T>(DataAccess access, T paramsObject)
		{
			string commandTextTemplate = "delete from {0} where {1} = '{2}'";
			string primaryKeyId = string.Empty;
			string primaryKeyValue = string.Empty;
			string tableName = string.Empty;
			IDbConnection connection = access.NewConnection;
			IDbCommand command = connection.CreateCommand();
			
			List<DataTableAttribute> dataTables =
				new List<DataTableAttribute>((TableAttribute[])typeof(T).GetCustomAttributes(typeof(TableAttribute), true));
			
			if(dataTables.Count > 0)
			{
				tableName = dataTables[0].Name;
			}
			
			foreach(PropertyInfo propertyInfo in typeof(T).GetProperties())
			{	
				List<KeyFieldAttribute> keyFields =
					new List<KeyFieldAttribute>((KeyFieldAttribute[])propertyInfo.GetCustomAttributes(typeof(KeyFieldAttribute), true));
				
				if(keyFields.Count > 0)
				{
					primaryKeyId = keyFields[0].Name;
					primaryKeyValue = propertyInfo.GetValue(paramsObject, null).ToString();
					break;
				}
			}
			
			command.CommandText = string.Format(
			                                    commandTextTemplate,
			                                    tableName,
			                                    primaryKeyId,
			                                    primaryKeyValue);
			command.CommandType = CommandType.Text;
			return command;
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
