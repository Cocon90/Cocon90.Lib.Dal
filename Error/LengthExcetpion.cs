using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Error
{
    /// <summary>
    /// 长度不一致引发的异常
    /// </summary>
    [Serializable]
    public class LengthExcetpion : System.Exception
    {
        public LengthExcetpion() { }
        public LengthExcetpion(string message) : base(message) { }
        public LengthExcetpion(string message, System.Exception inner) : base(message, inner) { }
        protected LengthExcetpion(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
