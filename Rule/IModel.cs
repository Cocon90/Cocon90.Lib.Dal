using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 标记接口，表示此类是模型类
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// 获取此模型对应的表名（用于增删改查）
        /// </summary>
        string TableName { get; }
    }
}
