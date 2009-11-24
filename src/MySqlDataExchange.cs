
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace ooglue
{
	public class MySqlDataExchange : IDataExchange
	{
		public MySqlDataExchange ()
		{
		}
		
		public T GetFromDataReader<T>(IDataReader reader) where T : new()
		{
			if(!(reader is MySqlDataReader))
			{
				throw new InvalidCastException(string.Format("The {0} requires a data reader of type {1}.", this.GetType().Name, typeof(MySqlDataReader).Name));
			}
			
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
					PropertyInfo pi = currentT.GetType().GetProperty(member.Name);
					object [] attributes = member.GetCustomAttributes(typeof(DataColumnAttribute), true);
					foreach(DataAttribute attr in attributes)
					{
						if(attr.Name == member.Name) //we've got a hit Captain
						{
							pi.SetValue(currentT, new object(). new object[]);
						}
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
