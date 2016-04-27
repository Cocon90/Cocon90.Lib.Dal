using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 存放实体属性的常用临时变量值
    /// </summary>
    public class AttributeParam
    {
        private static string mainTableAlisa = null;
        /// <summary>
        /// 取得类对像对应的主表的别名。（程序每启动一次，生成一次新的别名：mt_前8位Guid值）
        /// </summary>
        public static string MainTableAlisa { get { if (mainTableAlisa == null) { mainTableAlisa = "mt_" + Guid.NewGuid().ToString("N").Substring(0, 8); } return mainTableAlisa; } }
    }
}
