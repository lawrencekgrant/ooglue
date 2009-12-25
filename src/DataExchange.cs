
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
				List<Attribute> attribs = new List<Attribute>((Attribute[])pInfo.GetCustomAttributes(typeof(DataColumnAttribute), true));
				foreach(DataColumnAttribute dcAttr in attribs)
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
					new List<DataColumnAttribute>((DataColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(DataColumnAttribute), true));
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
				new List<DataTableAttribute>((DataTableAttribute[])typeof(T).GetCustomAttributes(typeof(DataTableAttribute), true));
			
			IDbConnection connection = access.NewConnection;
			IDbCommand command = connection.CreateCommand();
			
			if(dataTableAttributes.Count > 0)
			{
				dataTableName = dataTableAttributes[0].Name;
			}
			foreach(PropertyInfo propertyInfo in typeof(T).GetProperties())
			{				
				List<DataColumnAttribute> columnAttributes = 
					new List<DataColumnAttribute>((DataColumnAttribute[])propertyInfo.GetCustomAttributes(typeof(DataColumnAttribute), true));
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
		
		
		/*
		 * This may not be the right thing to do in this class. This probably isn't the place to interact with SQL
		 * 
		public static T GetObjectFromMethod<T>(DataAccess access, MethodBase method) where T : new()
		{
			List<DataMethodAttribute> methodAttributes = new List<DataMethodAttribute>((DataMethodAttribute[])method.GetCustomAttributes(typeof(DataMethodAttribute), true));
			foreach(DataMethodAttribute dma in methodAttributes)
			{
				IDbConnection connection = null;
				IDataReader reader = null;
				
				try
				{
					connection = access.NewConnection;
					reader = access.ExecuteReader(connection, dma.OutputMethod, GetDataParametersFromInfo(method.GetParameters()));
					connection.Open();
					return (T)GetFromDataReader<T>(reader, 1)[0];
				}
				catch(Exception ex)
				{
					Debug.WriteLine(ex.ToString());
					throw;
				}
				finally
				{
					if(reader != null)
						reader.Close();
					if(connection != null)
						connection.Close();
				}
				
			}
			return default(T);
		}
		*/
		
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
