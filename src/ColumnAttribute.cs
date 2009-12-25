
using System;

namespace ooglue
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class ColumnAttribute : DataAttribute
	{
		public bool IsPrimaryKey { get; set; }

		public ColumnAttribute (string name) : base(name)
		{
		}

		public ColumnAttribute (string name, bool isPrimaryKey) : this(name)
		{
			this.IsPrimaryKey = isPrimaryKey;
		}
	}
}
