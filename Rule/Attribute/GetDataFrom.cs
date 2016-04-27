using Cocon90.Lib.Dal.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cocon90.Lib.Dal.Rule.Attribute
{
    /// <summary>
    /// <para>获取数据的方法，其中 tableAlisa为空的含义是：当IsTableUnit为True时，tableAlisa为空，则自动取胜主表别名。当IsTableUnit为False时，tableAlisa为空，则自动取随机别名。</para>
    /// <para>tableName：传入所在表的表名（若为空表示多表联合本类表查询）；</para> 
    /// <para>tableAlias：如果是多个相同的表的查询，可以使用不同的别名来区分，tableAlisa为空的含义是：当IsTableUnit为True时，tableAlisa为空，则自动取胜主表别名。当IsTableUnit为False时，tableAlisa为空，则自动取随机别名。；</para>
    /// <para>columnName：对应的列名，若此项为空，则系统自动使用对应属性名称作为列名；</para>
    /// <para>tableColumnAsEqual_left：目标数据表中的列（作为“=”号左侧条件）；</para>
    /// <para>currentColumnAsEqual_right：当前类对象对应的数据表中的列（作为“=”号右侧条件）；</para>
    /// <para>isTableUnit：表示当前属性是否要使用多表联合查询还是赋值查询。True表示使用多表联合查询,False表示使用赋值查询。需要注意的是：如果此值为False，则当tableAlias为空时，系统将会自动赋予随机数做别名。</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GetDataFrom : System.Attribute
    {
     
        /// <summary>
        /// 所在的表名
        /// </summary>
        public string TableName { get; internal set; }
        /// <summary>
        /// 所在的表名的别名（可以为NULL）
        /// </summary>
        public string TableAlias { get; internal set; }
        /// <summary>
        /// 数据代表的列名
        /// </summary>
        public string ColumnName { get; internal set; }
        /// <summary>
        /// 条件，左侧条件列名
        /// </summary>
        public string TableColumnAsEqual_left { get; internal set; }
        /// <summary>
        /// 条件，右侧条件列名
        /// </summary>
        public string CurrentColumnAsEqual_right { get; internal set; }
        /// <summary>
        /// 当前属性是否要使用多表联合查询还是赋值查询。True表示使用多表联合查询 ，False表示使用赋值查询，使用赋值查询时，将查询关系中的第1个值赋值给宿主属性，如：select className,teacherName,(select top 1 count(*) from SuudentTab) studentCount from ClassTab。需要注意的是：如果此值为False，则当tableAlias为空时，系统将会自动赋予随机数做别名。
        /// </summary>
        public bool IsTableUnit { get; internal set; }

        /// <summary>
        /// <para>获取数据的方法，其中 tableAlisa为空的含义是：当IsTableUnit为True时，tableAlisa为空，则自动取胜主表别名。当IsTableUnit为False时，tableAlisa为空，则自动取随机别名。</para>
        /// <para>tableName：传入所在表的表名（若为空表示多表联合本类表查询）；</para> 
        /// <para>tableAlias：如果是多个相同的表的查询，可以使用不同的别名来区分，tableAlisa为空的含义是：当IsTableUnit为True时，tableAlisa为空，则自动取胜主表别名。当IsTableUnit为False时，tableAlisa为空，则自动取随机别名。；</para>
        /// <para>columnName：对应的列名，若此项为空，则系统自动使用对应属性名称作为列名；</para>
        /// <para>tableColumnAsEqual_left：目标数据表中的列（作为“=”号左侧条件）；</para>
        /// <para>currentColumnAsEqual_right：当前类对象对应的数据表中的列（作为“=”号右侧条件）；</para>
        /// <para>isTableUnit：表示当前属性是否要使用多表联合查询还是赋值查询。True表示使用多表联合查询,False表示使用赋值查询。需要注意的是：如果此值为False，则当tableAlias为空时，系统将会自动赋予随机数做别名。</para>
        /// </summary>
        /// <param name="tableName">传入所在表的表名(若为空表示多表联合本类表查询)</param>
        /// <param name="tableAlias">如果是多个相同的表的查询，可以使用不同的别名来区分</param>
        /// <param name="columnName">对应的列名，若此项为空，则系统自动使用对应属性名称作为列名</param>
        /// <param name="tableColumnAsEqual_left">目标数据表中的列（作为“=”号右侧条件）</param>
        /// <param name="currentColumnAsEqual_right">当前类对象对应的数据表中的列（作为“=”号左侧条件）</param>
        /// <param name="isTableUnit">当前属性是否要使用多表联合查询还是赋值查询。True表示使用多表联合查询, ，False表示使用赋值查询，使用赋值查询时，将查询关系中的第1个值赋值给宿主属性，如：select className,teacherName,(select top1 count(*) from SuudentTab) studentCount from ClassTab。 需要注意的是：如果此值为False，则当tableAlias为空时，系统将会自动赋予随机数做别名。</param>
        public GetDataFrom(string tableName, string tableAlias, string columnName, string tableColumnAsEqual_left, string currentColumnAsEqual_right, bool isTableUnit = true)
        {
            this.TableName = tableName;
            this.TableAlias = tableAlias;
            this.ColumnName = columnName;
            this.TableColumnAsEqual_left = tableColumnAsEqual_left;
            this.CurrentColumnAsEqual_right = currentColumnAsEqual_right;
            this.IsTableUnit = isTableUnit;
            if (!isNameOk(tableName))
            {
                throw new ModelAttrbuteException("tableName的值：" + tableName + "，不符合书写规范！请使用以字母、下划线开头的，且由数字、字母、下划线组成的变量名");
            }
            if (!isNameOk(tableAlias))
            {
                throw new ModelAttrbuteException("tableAlias的值：" + tableAlias + "，不符合书写规范！请使用以字母、下划线开头的，且由数字、字母、下划线组成的变量名");
            }
            if (!isNameOk(columnName))
            {
                throw new ModelAttrbuteException("columnName的值：" + columnName + "，不符合书写规范！请使用以字母、下划线开头的，且由数字、字母、下划线组成的变量名");
            }
            if (!isNameOk(tableColumnAsEqual_left))
            {
                throw new ModelAttrbuteException("tableColumnAsEqual_left的值：" + tableColumnAsEqual_left + "，不符合书写规范！请使用以字母、下划线开头的，且由数字、字母、下划线组成的变量名");
            }
            if (!isNameOk(currentColumnAsEqual_right))
            {
                throw new ModelAttrbuteException("currentColumnAsEqual_right的值：" + currentColumnAsEqual_right + "，不符合书写规范！请使用以字母、下划线开头的，且由数字、字母、下划线组成的变量名");
            }

        }
        /// <summary>
        /// <para>获取数据的方法，此重载表示IsTableUnit为False（使用赋值查询）。系统将tableAlisa自动定义为随机别名。</para>
        /// <para>tableName：传入所在表的表名（若为空表示多表联合本类表查询）；</para> 
        /// <para>columnName：对应的列名，若此项为空，则系统自动使用对应属性名称作为列名；</para>
        /// <para>tableColumnAsEqual_left：目标数据表中的列（作为“=”号左侧条件）；</para>
        /// <para>currentColumnAsEqual_right：当前类对象对应的数据表中的列（作为“=”号右侧条件）；</para>
        /// </summary>
        /// <param name="tableName">传入所在表的表名(若为空表示多表联合本类表查询)</param>
        /// <param name="columnName">对应的列名，若此项为空，则系统自动使用对应属性名称作为列名</param>
        /// <param name="tableColumnAsEqual_left">目标数据表中的列（作为“=”号右侧条件）</param>
        /// <param name="currentColumnAsEqual_right">当前类对象对应的数据表中的列（作为“=”号左侧条件）</param>
        public GetDataFrom(string tableName, string columnName, string tableColumnAsEqual_left, string currentColumnAsEqual_right)
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
            this.TableColumnAsEqual_left = tableColumnAsEqual_left;
            this.CurrentColumnAsEqual_right = currentColumnAsEqual_right;
            this.IsTableUnit = false;
            if (!isNameOk(tableName))
            {
                throw new ModelAttrbuteException("tableName的值：" + tableName + "，不符合书写规范！请使用以字母、下划线开头的，且由数字、字母、下划线组成的变量名");
            }
            if (!isNameOk(columnName))
            {
                throw new ModelAttrbuteException("columnName的值：" + columnName + "，不符合书写规范！请使用以字母、下划线开头的，且由数字、字母、下划线组成的变量名");
            }
            if (!isNameOk(tableColumnAsEqual_left))
            {
                throw new ModelAttrbuteException("tableColumnAsEqual_left的值：" + tableColumnAsEqual_left + "，不符合书写规范！请使用以字母、下划线开头的，且由数字、字母、下划线组成的变量名");
            }
            if (!isNameOk(currentColumnAsEqual_right))
            {
                throw new ModelAttrbuteException("currentColumnAsEqual_right的值：" + currentColumnAsEqual_right + "，不符合书写规范！请使用以字母、下划线开头的，且由数字、字母、下划线组成的变量名");
            }

        }
        /// <summary>
        /// 检测名称是否合法
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool isNameOk(string name)
        {
            return !(name != null && name.Length > 0 && !(Char.IsLetter(name[0]) || name[0] == '_'));
        }
        /// <summary>
        /// 简易描述
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "表名别名：" + TableName + " " + TableAlias + "     选择列：" + ColumnName + "     条件：" + TableAlias + "." + TableColumnAsEqual_left + "=" + CurrentColumnAsEqual_right;
        }
        /// <summary>
        /// 取得表名和别名，中间用空格隔开，如：CaseTab ct
        /// </summary>
        /// <returns></returns>
        public string GetTableNameWithAlisa()
        {
            return this.TableName + " " + this.TableAlias;
        }
        /// <summary>
        /// 判断表名是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsTabNameIsNull()
        {
            if (TableName == null || TableName.Trim() == "") { return true; }
            else return false;
        }
        /// <summary>
        /// 判断表的别名是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsTabAlisaIsNull()
        {
            if (TableAlias == null || TableAlias.Trim() == "") { return true; }
            else return false;
        }
        /// <summary>
        /// 判断全名是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsColumnNameIsNull()
        {
            if (ColumnName == null || ColumnName.Trim() == "") { return true; }
            else return false;
        }
        /// <summary>
        /// 判断左条件是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsColumnAsEqual_leftIsNull()
        {
            if (TableColumnAsEqual_left == null || TableColumnAsEqual_left.Trim() == "") { return true; }
            else return false;
        }
        /// <summary>
        /// 判断右条件是否为空
        /// </summary>
        /// <returns></returns>
        public bool IsColumnAsEqual_rightIsNull()
        {
            if (CurrentColumnAsEqual_right == null || CurrentColumnAsEqual_right.Trim() == "") { return true; }
            else return false;
        }
    }

}
