
using System;

namespace ooglue
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class DataMethodAttribute : DataAttribute
	{
		public string InputMethod {get;set;}
		public string OutputMethod {get;set;}
		
		public DataMethodAttribute(string inputMethodName, string outputMethodName)
		{
			InputMethod = inputMethodName;
			OutputMethod = outputMethodName;
		}
	}
}
