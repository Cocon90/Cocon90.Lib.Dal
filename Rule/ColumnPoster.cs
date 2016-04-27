using Cocon90.Lib.Dal.Rule.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 临时类
    /// </summary>
    internal class ColumnPoster
    {
        /// <summary>
        /// 实际最终要返回的列的列名(不包括别名)
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 数据获取的表中的相关信息
        /// </summary>
        public GetDataFrom DataFromAttribute { get; set; }
    }
}
