
using System;

namespace ooglue
{
	/// <summary>
	/// Represents a table.
	/// <remarks>
	/// 		This object may have two uses. The first is to specify a structure of which an object belongs to. The second use of this object
	/// is to give the object the behavior of a table, or more appropriately in this case a structure containing a list of typed objects.
	/// </remarks>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class TableAttribute : DataAttribute
	{		
		/// <summary>
		/// Creates a new TableAttribute.
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/> containing the name of the table of which this Attribute represents.
		/// </param>
		public TableAttribute(string name) : base(name) { }
	}
}
