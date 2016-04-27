using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule.Attribute
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ColumnAttribute : System.Attribute
    {

        public bool IsPrimaryKey { get; set; }
        public bool IsNotNull { get; set; }
        public string DefaultValue { get; set; }
    }
}
