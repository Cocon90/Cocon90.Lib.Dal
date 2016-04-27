using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Error
{
    /// <summary>
    /// 初始化错误
    /// </summary>
    [Serializable]
    public class InitException : Exception
    {
        public InitException() { }
        public InitException(string message) : base(message) { }
        public InitException(string message, Exception inner) : base(message, inner) { }
        protected InitException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
