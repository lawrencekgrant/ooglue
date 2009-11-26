
using System;
using System.Data;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ooglue
{
	public class DataExchange
	{
		public DataExchange ()
		{
		}
		
		public static T GetFromDataReader<T>(IDataReader reader) where T : new()
		{
			return GetFromDataReader<T>(reader, 1)[0];
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
			
			while(reader.Read() && counter++ < count)
			{
				T t = new T();
				string fieldName = string.Empty;
				for(int i = 0; i < reader.FieldCount; i++)
				{
					fieldName = reader.GetName(i);
					if(propertyMap.ContainsKey(fieldName))
					{
						//assign the fucking property value.
						PropertyInfo info = propertyMap[fieldName];
						info.SetValue(t, reader.GetValue(i), null);
					}
				}
				returnList.Add(t);
			}
			Debug.WriteLine(returnList.ToString());
			return returnList;
		}
		
		public static T GetObjectFromMethod<T>(DataAccess access, MethodInfo method) where T : new()
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
					
		private static IDataParameter[] GetDataParametersFromInfo(ParameterInfo [] parameters)
		{
			List<IDataParameter> returnParams = new List<IDataParameter>();
				
			foreach(ParameterInfo parameterInfo in parameters)
			{
				List<InAttribute> ins = new List<InAttribute>((InAttribute[])parameterInfo.GetCustomAttributes(typeof(InAttribute), true));
				foreach(InAttribute ina in ins)
				{
				
				}
			}
			return returnParams.ToArray();
		}
	}
}
