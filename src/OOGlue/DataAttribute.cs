
using System;

namespace ooglue
{
	[AttributeUsage(AttributeTargets.All)]
	public abstract class DataAttribute : Attribute
	{
		public String Name { get; set; }

		public DataAttribute ()
		{
		}

		public DataAttribute (string name) : this()
		{
			Name = name;
		}
	}
}
