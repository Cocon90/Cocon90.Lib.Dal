using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{

    internal class ColumnInfo
    {
        public bool IsPrimaryKey { get; set; }
        public bool IsNotNull { get; set; }
        public string DefaultValue { get; set; }
        public string Name { get; set; }
        public Type Type { get; set; }
      
    }
}
