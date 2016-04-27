using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Rule
{
    /// <summary>
    /// 表结构
    /// </summary>
    [Serializable]
    public class TableSchema
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 架构名
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// 所有的列
        /// </summary>
        public List<ColumnSchema> Columns { get { return columns; } set { columns = value; } }
        List<ColumnSchema> columns = new List<ColumnSchema>();
        public string ToMsSqlTableName()
        {
            return string.Format("{0}.[{1}]", SchemaName, TableName);
        }
        /// <summary>
        /// 判断两个表结构的表名是否相同。
        /// </summary>
        public static bool IsTableNameEqual(TableSchema a, TableSchema b)
        {
            if (a == b) return true;
            else if (a == null && b != null) { return false; }
            else if (a != null && b == null) { return false; }
            else
            {
                return string.Compare(a.TableName, b.TableName, true) == 0 && string.Compare(a.SchemaName, b.SchemaName, true) == 0;
            }
        }
    }
    /// <summary>
    /// 列结构
    /// </summary>
    [Serializable]
    public class ColumnSchema
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 列描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 长度 
        /// </summary>
        public int? Len { get; set; }
        /// <summary>
        /// 小数位数
        /// </summary>
        public int? DotLen { get; set; }
        /// <summary>
        /// 是否是标志列
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 是否是主键列
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否允许空
        /// </summary>
        public bool IsNullable { get; set; }
        /// <summary>
        /// 默认值 请包含Default关键字。
        /// </summary>
        public string DefaultValue { get; set; }
    }
}
