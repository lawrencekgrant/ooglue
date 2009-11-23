
using System;

namespace ooglue
{
	public class DataMethodAttribute : DataAttribute
	{
		public string InputMethod {get;set;}
		public string OutputMethod {get;set;}

		public DataMethodAttribute ()
		{
		}
		
		public DataMethodAttribute(string inputMethodName, string outputMethodName) : this()
		{
			InputMethod = inputMethodName;
			OutputMethod = outputMethodName;
		}
	}
}
