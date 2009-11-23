
using System;
using System.Data;
using System.Collections;

namespace ooglue
{
	public interface IDataExchange
	{
		T GetFromDataReader<T>(IDataReader reader) where T : new();
		
		T GetFromDataRow<T>(DataRow dataRow) where T : new();
		
		//List<T> GetFromEnumerable<T>(IEnumerable enumerable) where T : new();
		
		//List<T> GetFromDataTable<T>(DataTable dataTable) where T : new();
	}
}
