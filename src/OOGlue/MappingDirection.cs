using System;

namespace ooglue
{
	/// <summary>
	/// This enumerates the directions that data can be mapped. Currently we only support the first or last element in.
	/// </summary>
	/// <remarks>
	/// I believe that we'll be using an indexed approach to handle this in the future. Perhaps specifying precidence in the actual column
	/// attribute is the solution to this.
	/// </remarks>
	public enum MappingDirection
	{
		/// <summary>
		/// The first element that matches a value will be mapped, and will not be overridden.
		/// </summary>
		FirstIn,
		/// <summary>
		/// The last element that matches a value will be mapped.
		/// </summary>
		LastIn
	}
}
