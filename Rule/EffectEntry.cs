using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 受影响信息的实体
    /// </summary>
    public class EffectEntry
    {
        /// <summary>
        /// 成功执行数
        /// </summary>
        public int SuccessCount { get; set; }
        /// <summary>
        /// 执行失败数
        /// </summary>
        public int FailCount { get; set; }
        /// <summary>
        /// 所有执行数
        /// </summary>
        public int AllCount { get; set; }
    }
}
