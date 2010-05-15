
using System;
using System.Configuration;

namespace ooglue.Configuration
{


	public sealed class ooglueConfigurationSection : ConfigurationSection
	{
		
		private static ConfigurationPropertyCollection Properties {get;set;}
		
		private static ConfigurationProperty _dataAccessType = new ConfigurationProperty("DataAccessType", typeof(Type));

		public ooglueConfigurationSection ()
		{
		}
	}
}
