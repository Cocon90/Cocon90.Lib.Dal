using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 返回无用的列名
    /// </summary>
    public interface IUseless
    {
        IEnumerable<string> UselessProperty { get; }
    }
}
