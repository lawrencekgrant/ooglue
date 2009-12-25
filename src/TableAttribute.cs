
using System;

namespace ooglue
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class TableAttribute : DataAttribute
	{		
		public TableAttribute(string name) : base(name) { }
	}
}
