
using System;

namespace ooglue
{


	public class ooglueException : Exception
	{

		public ooglueException () : base()
		{
		}
		
		public ooglueException(string message) : base(message) 
		{
		}
		
		public ooglueException(string message, Exception innerException) : base(message, innerException) 
		{
		}
		
		public ooglueException(Exception innerException) : base(innerException.Message, innerException)
		{
		}
	}
}
