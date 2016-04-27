using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Error
{
    /// <summary>
    /// 实体属性前面的特性标记不正确时引发的异常。
    /// </summary>
    [Serializable]
    public class ModelAttrbuteException : Exception
    {
        public ModelAttrbuteException() { }
        public ModelAttrbuteException(string message) : base(message) { }
        public ModelAttrbuteException(string message, Exception inner) : base(message, inner) { }
        protected ModelAttrbuteException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
