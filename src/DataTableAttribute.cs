
using System;

namespace ooglue
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class DataTableAttribute : DataAttribute
	{		
		public DataTableAttribute(string name) : base(name) { }
	}
}
