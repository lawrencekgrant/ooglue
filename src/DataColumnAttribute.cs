
using System;

namespace ooglue
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class DataColumnAttribute : DataAttribute
	{
		public bool IsPrimaryKey { get; set; }
		
		public DataColumnAttribute(string name) : base(name) { }
		
		public DataColumnAttribute(string name, bool isPrimaryKey) : this(name)
		{
			this.IsPrimaryKey = isPrimaryKey;
		}
	}
}
