
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace ooglue
{
	public class DataExchange
	{
		public DataExchange ()
		{
		}
		
		public static T GetFromDataReader<T>(IDataReader reader) where T : new()
		{
			T t = new T();
			Type tType = typeof(T);
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
			
			if(reader.Read())
			{
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
			}
			System.Diagnostics.Debug.WriteLine(t.ToString());
			return t;
		}
	}
}
