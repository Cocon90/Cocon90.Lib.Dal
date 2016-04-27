using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 参数
    /// </summary>
    [Serializable]
    [DataContract]
    public class Parameter
    {
        public Parameter() { }
        public Parameter(string name, object value)
        { this.Name = name; this.Value = value; }
        /// <summary>
        /// 参数键.请用@开头。
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 参数值.
        /// </summary>
        [DataMember]
        public object Value { get; set; }
        public override string ToString()
        {
            return string.Format("{0}={1}", Name, Value);
        }
        /// <summary>
        /// 定义由DbParameter到Parameter的隐式转换
        /// </summary>
        /// <param name="dbParameter"></param>
        /// <returns></returns>
        public static implicit operator Parameter(System.Data.Common.DbParameter dbParameter)
        {
            return new Parameter(dbParameter.ParameterName, dbParameter.Value);
        }
    }
}
