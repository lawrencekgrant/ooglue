
using System;

namespace ooglue
{
	/// <summary>
	/// The DataAttribute class is an abstract class that defines the base behavior of a data based attribute in ooglue.
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public abstract class DataAttribute : Attribute
	{
		/// <summary>
		/// The name of the attribute. 
		/// </summary>
		public String Name { get; set; }

		/// <summary>
		/// Creates a new DataAttribute object.
		/// </summary>
		public DataAttribute ()
		{
		}

		/// <summary>
		/// Creates a new instance of the object while specifying its name.
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/> containing the name of the newly created Attribute.
		/// </param>
		public DataAttribute (string name) : this()
		{
			Name = name;
		}
	}
}
