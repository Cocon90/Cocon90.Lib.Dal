using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 传替参数时使用的实体，包括名称和别名
    /// </summary>
    public class AliseEntry
    {
        /// <summary>
        /// 真实名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 表示别名
        /// </summary>
        public string Alise { get; set; }
    }
}
