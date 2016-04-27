using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Error
{
    /// <summary>
    /// 创建实例时引发的异常
    /// </summary>
    [Serializable]
    public class InstenceException : Exception
    {
        public InstenceException() { }
        public InstenceException(string message) : base(message) { }
        public InstenceException(string message, Exception inner) : base(message, inner) { }
        protected InstenceException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
