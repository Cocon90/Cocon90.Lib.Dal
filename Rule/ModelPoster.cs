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
    internal class ModelPoster
    {
        /// <summary>
        /// 实际最终要返回的实体中的属性
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 实体数据的获取方式属性。
        /// </summary>
        public GetModelFrom ModelFromAttribute { get; set; }
        /// <summary>
        /// 属性的类型
        /// </summary>
        public Type PropertyType { get; set; }
    }
}
