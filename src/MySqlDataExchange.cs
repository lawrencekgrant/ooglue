
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ooglue
{
	public class MySqlDataExchange : IDataExchange
	{
		public MySqlDataExchange ()
		{
		}
		
		public T GetFromDataReader<T>(IDataReader reader) where T : new()
		{
			T t = new T();
			return t;
		}
		
		public T GetFromDataRow<T>(DataRow dataRow) where T : new()
		{
			List<PropertyInfo> properties = new List<PropertyInfo>();
			
			T currentT = new T();
			Type type = typeof(T);
			MemberInfo [] members = type.GetMembers();
			foreach(MemberInfo member in members)
			{
				if(member.MemberType == MemberTypes.Property)
				{
					object [] attributes = member.GetCustomAttributes(typeof(DataColumnAttribute), true);
					foreach(DataAttribute attr in attributes)
					{
						
					}
				}
			}
			List<DataColumn> columns = new List<DataColumn>();
			for(int i = 0 ; i < dataRow.ItemArray.Length ; i++)
			{
				columns.Add((DataColumn)dataRow[i]);
			}
			
			
			
			return new T();
		}		
	}
}
