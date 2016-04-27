using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    [Serializable]
    [DataContract]
    public class SqlBatch
    {
        [DataMember]
        public string Sql { get; set; }
        [DataMember]
        public Parameter[] Params { get; set; }
    }
}
