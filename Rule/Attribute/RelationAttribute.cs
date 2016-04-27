using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule.Attribute
{
    /// <summary>
    /// 关系实体
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class RelationAttribute : System.Attribute
    {

        /// <summary>
        /// 构建一个Relation标记。
        /// </summary>
        public RelationAttribute(string DbFrom, Direction direction)
        {
            this.DbFrom = DbFrom;
            this.Direction = direction;
        }

        /// <summary>
        /// 获取或设置 向下查找时，此值为本表关系列名称. 当向上查找时，此值为从表的关系列名称。
        /// </summary>
        /// <value>
        public string DbFrom { get; set; }
        /// <summary>
        /// 获取或设置 执行方向，当向上查找时，相应列应该为实体类集合，向下查找时，相应列应该为实体类。
        /// </summary>
        public Direction Direction { get; set; }
    }
    /// <summary>
    /// 属性相应方向，向下或向上。
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// 向下查找 (向下查找时，相应列应该为实体类，可进行增改)
        /// </summary>
        ToDown,
        /// <summary>
        /// 向上查找 (向上查找时，相应列应该为实体类集合，不可进行增改)
        /// </summary>
        ToUp
    }
}
