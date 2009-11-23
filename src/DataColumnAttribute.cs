
using System;

namespace ooglue
{
	public class DataColumnAttribute : DataAttribute
	{

		public DataColumnAttribute ()
		{
		}
		
		public DataColumnAttribute (string name) : this()
		{
			this.Name = name;
		}
	}
}
