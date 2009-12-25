
using System;

namespace ooglue
{
	public class DataColumnAttribute : DataAttribute
	{
		public bool IsPrimaryKey { get; set; }
		
		public DataColumnAttribute(string name) : base(name) { }
		
		public DataColumnAttribute(string name, bool isPrimaryKey) : this(name)
		{
			this.IsPrimaryKey = isPrimaryKey;
		}
	}
}
