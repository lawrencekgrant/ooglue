
using System;

namespace ooglue
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class KeyFieldAttribute : DataAttribute
	{
		public KeyFieldAttribute (string name) : base(name) { }
	}
}
