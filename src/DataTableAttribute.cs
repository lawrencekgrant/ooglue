
using System;

namespace ooglue
{
	public class DataTableAttribute : DataAttribute
	{
		public DataTableAttribute ()
		{
		}
		
		public DataTableAttribute(string name) : this()
		{
			Name = name;
		}
	}
}
