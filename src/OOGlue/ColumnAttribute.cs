
using System;

namespace ooglue
{
	/// <summary>
	/// This attribute is used to tie property or field to a particular column name.
	/// </summary>
	/// <remarks>
	/// A ColumnAttribute may only be applied to a field or an attribute. ColumnAttributes are used in both data mapping and object mapping.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class ColumnAttribute : DataAttribute
	{
		/// <summary>
		/// The ooglue system uses this property to determine whether a property is a primary key column. This is used heavily in selecting
		/// and updating values.
		/// </summary>
		public bool IsPrimaryKey { get; set; }

		/// <summary>
		/// Constructs a new ColumnAttribute
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/> that represents the name of the column to be bound to.
		/// </param>
		public ColumnAttribute (string name) : base(name)
		{
		}

		/// <summary>
		/// Constructs a new ColumnAttribute
		/// </summary>
		/// <param name="name">
		/// A <see cref="System.String"/> that represents the name of the column to be bound to.
		/// </param>
		/// <param name="isPrimaryKey">
		/// A <see cref="System.Boolean"/> that determines whether the specified column is a primary key column or not.
		/// </param>
		public ColumnAttribute (string name, bool isPrimaryKey) : this(name)
		{
			this.IsPrimaryKey = isPrimaryKey;
		}
	}
}
