
using System;
using NUnit.Framework;
using System.Reflection;
using System.Configuration;

using ooglue;
using ooglue.Access;

namespace ooglue.test
{
	[Table("TestThing")]
	public class TestThing
	{
		[Column("Name")]
		public string Name {get;set;}
		
		[Column("Description")]
		public string Description {get;set;}
	}


	[TestFixture()]
	public class GeneralTests
	{

		[Test()]
		public void TestCase ()
		{
			Console.WriteLine("Starting generic test case.");
			Console.WriteLine("Value = {0}", ConfigurationManager.AppSettings["key"]);
			Console.WriteLine("Executing Application Path: {0}", Assembly.GetExecutingAssembly().Location);
			ooglue.DataConveyor conveyor = new ooglue.DataConveyor(new MySqlAccess());
			ooglue.DataExchange exchange = new ooglue.DataExchange(new MySqlAccess());
			
			TestThing newTestThing = new TestThing(){Name = "name", Description = "desc"};
			string sqlString = exchange.GetDynamicInsertStringFromObject<TestThing>(newTestThing);
			Console.WriteLine(sqlString);
			Assert.IsNotEmpty(sqlString);
		}
	}
}
