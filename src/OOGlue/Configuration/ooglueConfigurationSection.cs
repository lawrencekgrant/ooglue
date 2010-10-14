
using System;
using System.Configuration;

namespace ooglue.Configuration
{


	public sealed class OOGlueConfigurationSection : ConfigurationSection
	{
		
		private static ConfigurationPropertyCollection Properties {get;set;}
		
		private static ConfigurationProperty _dataAccessType = new ConfigurationProperty("DataAccessType", typeof(Type));
		
		[ConfigurationProperty("connectionString", IsRequired = true)]
		public string ConnectionString {get; set;}
		
		[ConfigurationProperty("dataAccessTypeName", IsRequired = true)]
		public Type DataAccessTypeName {get;set;}

		public OOGlueConfigurationSection ()
		{
		}
	}
}
