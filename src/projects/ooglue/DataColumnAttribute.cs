/*
 * Created by SharpDevelop.
 * User: dev
 * Date: 1/31/2008
 * Time: 1:13 AM
 * 
 */
using System;
using System.Collections.Generic;

namespace ooglue
{
	/// <summary>
	/// Description of DataColumnAttribute
	/// </summary>
	public class DataColumnAttribute : Attribute
	{
        public string ColumnName { get; set; }

        public DataColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
	}
}
