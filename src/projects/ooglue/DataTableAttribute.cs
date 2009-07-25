using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Text;

namespace ooglue
{
    public class DataTableAttribute
    {
        public string DataTableName { get; set; }

        public DataTableAttribute(string dataTableName)
        {
            this.DataTableName = dataTableName;
        }
    }
}
