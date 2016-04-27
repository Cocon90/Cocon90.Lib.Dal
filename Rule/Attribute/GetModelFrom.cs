using Cocon90.Lib.Dal.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cocon90.Lib.Dal.Rule.Attribute
{
    /// <summary>
    /// 取得实体集合数据。使用此标记的属性的类型必须实现ISetModel接口。常用的有ModelData<T>类。
    /// <para>tableName:查询时要联合的表的表名</para>
    /// <para>orderBy:查询时要联合的表的排序列名</para>
    /// <para>isAsc:查询时，排序列是按正顺(true)还是倒序(false)</para>
    /// <para>tableColumnAsEqual_left:查询时，要联合表中所对应的列名，作为等号左条件。</para>
    /// <para>currentColumnAsEqual_right:查询时，要当前绑定实体中所对应的属性名，作为等号右条件。</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GetModelFrom : System.Attribute
    {

        /// <summary>
        /// 获取或设置 查询时要联合的表的表名
        /// </summary>
        internal string TableName { get; set; }
        /// <summary>
        /// 获取或设置 查询时要联合的表的排序列名
        /// </summary>
        internal string OrderBy { get; set; }
        /// <summary>
        /// 获取或设置 查询时，排序列是按正顺(true)还是倒序(false)
        /// </summary>
        internal bool IsAsc { get; set; }
        /// <summary>
        /// 获取或设置 查询时，要联合表中所对应的列名，作为等号左条件。
        /// </summary>
        internal string TableColumnAsEqual_left { get; set; }
        /// <summary>
        /// 获取或设置 查询时，要当前绑定实体中所对应的属性名，作为等号右条件。
        /// </summary>
        internal string CurrentPropertyAsEqual_right { get; set; }

        /// <summary>
        /// 取得实体集合数据。 
        /// </summary>
        /// <param name="tableName">查询时要联合的表的表名</param>
        /// <param name="orderBy">查询时要联合的表的排序列名</param>
        /// <param name="isAsc">查询时，排序列是按正顺(true)还是倒序(false)</param>
        /// <param name="tableColumnAsEqual_left">查询时，要联合表中所对应的列名，作为等号左条件。</param>
        /// <param name="currentPropertyAsEqual_right">查询时，要当前绑定实体中所对应的属性名，作为等号右条件。</param>
        public GetModelFrom(string tableName, string orderBy, bool isAsc, string tableColumnAsEqual_left, string currentPropertyAsEqual_right)
        {
            this.TableName = tableName;
            this.OrderBy = orderBy;
            this.IsAsc = isAsc;
            this.TableColumnAsEqual_left = tableColumnAsEqual_left;
            this.CurrentPropertyAsEqual_right = currentPropertyAsEqual_right;
        }

        /// <summary>
        /// 取得当前的查询语句。右侧值条件，将自动使用“@currentPropertyAsEqual_right”。返回的语句如：select * from unitTab where unitId=@currentPropertyAsEqual_right order by CTime desc
        /// </summary>
        /// <param name="mainModel"></param>
        /// <returns></returns>
        public string getSelectSql()
        {
            var sql = String.Format("select {0} from {1} where {2}={3} order by {4} {5}",
                 "*",
                this.TableName,
                TableColumnAsEqual_left,
                "@currentPropertyAsEqual_right",
                OrderBy,
                IsAsc ? " asc " : " desc "
                );
            return sql;
        }
    }

}
