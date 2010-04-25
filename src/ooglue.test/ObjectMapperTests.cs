
using System;
using NUnit.Framework;

using log4net;

namespace ooglue.test
{

	/// <summary>
	/// These tests ensure that the object mapper continues to perform as expected as changes are made to its internal structure. Also,
	/// like most other tests, this ensures that the contracts have not changed in the use of the object. Lastly, I'll add some minor
	/// performance tests to this class. 
	/// </summary>
	[TestFixture()]
	public partial class ObjectMapperTests
	{
		
		ObjectMapper mapper;
		TypeOne typeOne;
		TypeTwo typeTwo;
		TypeThree typeThree;
		
		DateTime timerStart;
		
		ILog log = LogManager.GetLogger(typeof(ObjectMapperTests));
		
		[SetUp]
		public void SetUp()
		{
			//let's set up our mapper....
			mapper = new ObjectMapper();
			
			//then we can construct our test objects.
			 
			typeOne = new TypeOne() {PropertyOne = "one", PropertyTwo="two"};
			typeTwo = new TypeTwo() {PropertyOne = "1", PropertyTwo="2"};
			typeThree = new TypeThree() {PropertyOne = "p1", PropertyTwo="p2", PropertyThree="p3"};
		}
		
		[TearDown]
		public void TearDown()
		{
			//we really could let garbage collection take care of this as it's a lightweight object. Even so, we'll go ahead and null out
			//the mapper when we tear down our fixture.
			mapper = null;
		}
		
		/// <summary>
		/// Here we'll map two objects together.
		/// </summary>
		[Test()]
		public void MapTwoObjectsFirstIn()
		{
			StartTimer();
			TypeThree returnObj = mapper.Map<TypeThree>(
			    MappingDirection.FirstIn,
				typeOne,
				typeThree			                                            
			);
			StopTimer();
			Assert.AreSame(returnObj.PropertyOne, typeOne.PropertyOne);
			Assert.AreSame(returnObj.PropertyTwo, typeOne.PropertyTwo);
			Assert.AreSame(returnObj.PropertyThree, typeThree.PropertyThree);
		}
		
		/// <summary>
		/// Here we map three objects together, first in wins.
		/// </summary>
		[Test()]
		public void MapThreeObjectsFirstIn()
		{
			StartTimer();
			TypeTwo returnObj = mapper.Map<TypeTwo>(
				MappingDirection.FirstIn,		
			    typeOne,
			    typeTwo,
				typeThree                           
			);
			StopTimer();
				
			Assert.AreSame(returnObj.PropertyOne,  typeOne.PropertyOne);
			Assert.AreSame(returnObj.PropertyTwo, typeOne.PropertyTwo);
		}
		
		/// <summary>
		/// This test maps three objects together, last in wins.
		/// </summary>
		[Test()]
		public void MapThreeObjectsLastIn()
		{
			StartTimer();
			TypeTwo returnObj = mapper.Map<TypeTwo>(
				MappingDirection.LastIn,			                                      
				typeOne,
				typeTwo,
				typeThree			                                        
			);
			StopTimer();
			
			Assert.AreSame(returnObj.PropertyOne, typeThree.PropertyOne);
			Assert.AreSame(returnObj.PropertyTwo, typeThree.PropertyTwo);
		}
		
		private void StartTimer()
		{
			timerStart = DateTime.Now;
			log.InfoFormat("Test started at {0}", timerStart);
		}
		
		private void StopTimer()
		{
			TimeSpan runningTestTime = DateTime.Now.Subtract(timerStart);
			log.InfoFormat("Test complete. Total runnning time {0}", runningTestTime);
		}
	}
}
