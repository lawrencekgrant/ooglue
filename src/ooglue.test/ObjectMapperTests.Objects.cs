
using System;

namespace ooglue.test
{


	public partial class ObjectMapperTests
	{

		/// <summary>
		/// This is an inner-class for testing the object mapper.
		/// </summary>
		public class TypeOne
		{
			[Column("Prop1")]
			public string PropertyOne {get;set;}
			
			[Column("Prop2")]
			public string PropertyTwo {get;set;}
		}
		
		/// <summary>
		/// This is an inner-class for testing the object mapper.
		/// </summary>
		public class TypeTwo
		{
			[Column("Prop1")]
			public string PropertyOne {get;set;}
			
			[Column("Prop2")]
			public string PropertyTwo {get;set;}
		}
		
		/// <summary>
		/// This is an inner-class for testing the object mapper.
		/// </summary>
		public class TypeThree
		{
			[Column("Prop1")]
			public string PropertyOne {get;set;}
			
			[Column("Prop2")]
			public string PropertyTwo {get;set;}
			
			[Column("Prop3")]
			public string PropertyThree {get;set;}
		}
	}
}
