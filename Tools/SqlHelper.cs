using Cocon90.Lib.Dal.Error;
using Cocon90.Lib.Dal.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Cocon90.Lib.Dal.Tools
{
    /// <summary>
    /// SQL语句辅助类，语句中用到的参数，请统一使用"@"符号。系统将自动为相应数据库引擎转换。
    /// </summary>
    public class SqlHelper
    {
        /// <summary>
        /// 获取或设置 当前SqlHelper中相关语句所使用的DataHelper
        /// </summary>
        public IDataHelper DataHelper { get; set; }
        /// <summary>
        /// 指定SqlHelper中相关语句所使用的DataHelper
        /// </summary>
        /// <param name="dataHelper"></param>
        public SqlHelper(IDataHelper dataHelper)
        {
            this.DataHelper = dataHelper;
        }
        /// <summary>
        /// 返回分页字符串 ，依次传入：源Sql语句（此Sql语句不能包含Order By），排序列名(此列名必须是源语句执行后展示出来的有效列名)，是否升序，显示页数（从1开始），第页数据量
        /// </summary>
        public string getPagedSql(string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize)
        {
            return getPagedSql(this.DataHelper, sourceSql, orderColumnName, isAsc, pageNumber, pageSize);
        }
        /// <summary>
        /// 返回分页字符串 ，依次传入相应的IDataHelper,源Sql语句（此Sql语句不能包含Order By），排序列名(此列名必须是源语句执行后展示出来的有效列名)，是否升序，显示页数（从1开始），第页数据量
        /// </summary>
        public string getPagedSql(IDataHelper dataHelper, string sourceSql, string orderColumnName, bool isAsc, int pageNumber, int pageSize)
        {
            var result = "";
            if (dataHelper == null || dataHelper is Utility.SQLDataHelper)
            {
                var rownum = "rownum" + DateTime.Now.Millisecond + "" + DateTime.Now.Second + "" + DateTime.Now.Minute;
                result = string.Format("select * from (select *,ROW_NUMBER() over (order by {0} " + (isAsc ? " Asc " : " Desc ") + ") " + rownum + " from ({1}) oldsqlstring" + DateTime.Now.Millisecond + "" + DateTime.Now.Second + "" + DateTime.Now.Minute + ") tab where tab." + rownum + " between ({2}-1)*{3}+1 and ({2})*{3}", orderColumnName, sourceSql, pageNumber, pageSize);
            }
            else if (dataHelper is Utility.SQLiteDataHelper || dataHelper is Utility.MySqlDataHelper)
            {
                result = string.Format("select * from ({0}) oldsqlstring order by {1} " + (isAsc ? "Asc" : "Desc") + " limit ({2}-1)*{3},{3}", sourceSql, orderColumnName, pageNumber, pageSize);
            }
            else if (dataHelper is Utility.PostgreSqlDataHelper)
            {
                result = string.Format("select * from ({0}) oldsqlstring order by {1} " + (isAsc ? "Asc" : "Desc") + " limit {3} offset ({2}-1)*{3} ", sourceSql, orderColumnName, pageNumber, pageSize);
            }
            else throw new NotSupportedException("此类型数据库（" + dataHelper.GetType().FullName + "）尚未支持！");
            return result;
        }
        /// <summary>
        /// 返回排序字符串(对子表查询语句进行二次排序)，传入指定排序列名、是否升序
        /// </summary>
        public string getOrderBySql(string sourceSql, string orderColumnName, bool isAsc)
        {
            return string.Format("select * from ({0}) OrderByTempTable" + DateTime.Now.Millisecond + "" + DateTime.Now.Second + "" + DateTime.Now.Minute + "  order by {1} {2}", sourceSql, orderColumnName, isAsc ? " Asc " : " Desc ");
        }

        /// <summary>
        /// 用于插入数据，自动将传和的参数按格式排列。如addSql("1", 2, 3.5f, DateTime.Now, true, null, DBNull.Value)，将返回'1',2,3.5,'2014-09-25 10:52:23',1,NULL,''。（如果值为bool，将自动将它换成数字0或1。）
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string getAddSql(params object[] value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in value)
            {
                if (item == null)
                {
                    sb.Append(String.Format("{0},", "NULL"));
                }
                else if (item is int || item is float || item is double)
                {
                    sb.Append(String.Format("{0},", item));
                }
                else if (item is bool)
                {
                    sb.Append(String.Format("{0},", ((bool)item) ? 1 : 0));//在SQLServer中可以直接使用'True'或1，可是在Sqlite中，只能用数字0或非0.
                }
                else if (item is DateTime)
                {
                    sb.Append(String.Format("'{0}',", ((DateTime)item).ToString("yyyy-MM-dd HH:mm:ss")));//为了兼容SqlLite就使用这样了，把毫秒给扔了。
                }
                else
                {
                    sb.Append(String.Format("'{0}',", item));
                }
            }
            return sb.ToString().TrimEnd(',');
        }
        /// <summary>
        /// 自动生成"@Column1,@Columns2,@Column3"字符串
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string getAddSqlParam(List<string> columnName)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < columnName.Count; i++)
            {
                sb.Append(String.Format(this.DataHelper.ParamChar + "{0},", columnName[i]));
            }
            return sb.ToString().TrimEnd(',');
        }
        /// <summary>
        /// 用于修改数据，自动将传入的参数按格式排列。如传入 dic.Add("aaa", 1); dic.Add("bbb", "bbbb"); dic.Add("ccc", DateTime.Now); dic.Add("ddd", true); dic.Add("eee", DBNull.Value);，将返回aaa=1,bbb='bbbb',ccc='2014-09-25 13:54:58',ddd=1,eee=''。（如果值为bool，将自动将它换成数字0或1。）
        /// </summary>
        /// <param name="ColumnsAndValue"></param>
        /// <returns></returns>
        public string getEditSql(Dictionary<string, object> ColumnsAndValue)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in ColumnsAndValue.Keys)
            {
                if (ColumnsAndValue[item] == null)
                {
                    sb.Append(String.Format("{0}={1},", item, "NULL"));
                }
                else if (ColumnsAndValue[item] is int || ColumnsAndValue[item] is float || ColumnsAndValue[item] is double)
                {
                    sb.Append(String.Format("{0}={1},", item, ColumnsAndValue[item]));
                }
                else if (ColumnsAndValue[item] is bool)
                {
                    sb.Append(String.Format("{0}={1},", item, ((bool)ColumnsAndValue[item]) ? 1 : 0));//在SQLServer中可以直接使用'True'或1，可是在Sqlite中，只能用数字0或非0.
                }
                else if (ColumnsAndValue[item] is DateTime)
                {
                    sb.Append(String.Format("{0}='{1}',", item, ((DateTime)ColumnsAndValue[item]).ToString("yyyy-MM-dd HH:mm:ss")));//为了兼容SqlLite就使用这样了，把毫秒给扔了。
                }
                else
                {
                    sb.Append(String.Format("{0}='{1}',", item, ColumnsAndValue[item]));
                }
            }
            return sb.ToString().TrimEnd(',');
        }
        /// <summary>
        /// 用于修改数据，自动生成Column1=@Column1,Column2=@Column2,Column3=@Column3
        /// </summary>
        /// <param name="ColumnsAndValue"></param>
        /// <returns></returns>
        public string getEditSqlParam(List<string> columnName)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < columnName.Count; i++)
            {
                sb.Append(String.Format("{0}={1}{0},", columnName[i], DataHelper.ParamChar));
            }
            return sb.ToString().TrimEnd(',');
        }
        /// <summary>
        /// 用于条件语句数据，自动将传入的参数按格式排列。如： aaa=1 and bbb='bbbb' and ccc='2014-09-25 14:13:40' and ddd=1 and eee='' （如果值为bool，将自动换成0或1。） 参数isMeanNot表示是否将值意义取返。 比如aaa is null and bbb<>'bbb' and ccc<>'2014-09-25'
        /// </summary>
        /// <param name="ColumnsAndValue"></param>
        /// <returns></returns>
        public string getWhereSql(Dictionary<string, object> ColumnsAndValue, bool isMeanNot = false)
        {
            string equal = "=";
            if (isMeanNot) { equal = "<>"; }
            StringBuilder sb = new StringBuilder();
            foreach (var item in ColumnsAndValue.Keys)
            {
                if (ColumnsAndValue[item] == null)
                {
                    if (isMeanNot) { sb.Append(String.Format(" {0} is not null and", item)); }
                    else { sb.Append(String.Format(" {0} is null and", item)); }
                }
                else if (ColumnsAndValue[item] is int || ColumnsAndValue[item] is float || ColumnsAndValue[item] is double)
                {
                    sb.Append(String.Format(" {0}" + equal + "{1} and", item, ColumnsAndValue[item]));
                }
                else if (ColumnsAndValue[item] is bool)
                {
                    sb.Append(String.Format(" {0}" + equal + "{1} and", item, ((bool)ColumnsAndValue[item]) ? 1 : 0));//在SQLServer中可以直接使用'True'或1，可是在Sqlite中，只能用数字0或非0.
                }
                else if (ColumnsAndValue[item] is DateTime)
                {
                    sb.Append(String.Format(" {0}" + equal + "'{1}' and", item, ((DateTime)ColumnsAndValue[item]).ToString("yyyy-MM-dd HH:mm:ss")));//为了兼容SqlLite就使用这样了，把毫秒给扔了。
                }
                else
                {
                    sb.Append(String.Format(" {0}" + equal + "'{1}' and", item, ColumnsAndValue[item]));
                }
            }
            var sql = sb.ToString();
            sql = sql.Remove(sql.Length - 3);//去除结尾的and
            return sql;
        }
        /// <summary>
        /// 用于条件语句数据，传出模糊查询的语句如： XXX like '%abc%' and YYY is null
        /// </summary>
        /// <param name="ColumnsAndValue"></param>
        /// <returns></returns>
        public string getWhereLikeSql(Dictionary<string, object> ColumnsAndValue)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in ColumnsAndValue.Keys)
            {
                if (ColumnsAndValue[item] == null)
                {
                    sb.Append(String.Format(" {0} is null and", item));
                }
                else if (ColumnsAndValue[item] is bool)
                {
                    sb.Append(String.Format(" {0} {1} and", item, ((bool)ColumnsAndValue[item]) ? "<> 0 " : "=0"));//在SQLServer中可以直接使用'True'或1，可是在Sqlite中，只能用数字0或非0.
                }
                else if (ColumnsAndValue[item] is DateTime)
                {
                    sb.Append(String.Format(" {0} like '%{1}%' and", item, ((DateTime)ColumnsAndValue[item]).ToString("yyyy-MM-dd HH:mm:ss")));//为了兼容SqlLite就使用这样了，把毫秒给扔了。
                }
                else
                {
                    sb.Append(String.Format(" {0} like '%{1}%' and", item, ColumnsAndValue[item]));
                }
            }
            return sb.ToString().TrimEnd("and".ToCharArray());
        }
        /// <summary>
        /// 用于条件语句数据，自动生成"Column1=@Column1 and Column2=@Column2 and Column3=@Column3"
        /// </summary>
        /// <returns></returns>
        public string getWhereSqlParam(List<string> columnName)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < columnName.Count; i++)
            {
                sb.Append(String.Format(" {0}={1}{0} and", columnName[i], DataHelper.ParamChar));
            }
            return sb.ToString().TrimEnd("and".ToCharArray());
        }
        /// <summary>
        /// 用于条件语句数据，自动生成:
        /// "alise1.Column1=@alise1Column1 and alise1.Column2=@alise1Column2 and alise2.Column1=@alise2Column1"
        /// </summary>
        /// <returns></returns>
        public string getWhereSqlParam(List<AliseEntry> columnName)
        {
            if (columnName == null) return null;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < columnName.Count; i++)
            {
                sb.Append(String.Format(" {0}.{1}={3}{2} and", columnName[i].Alise, columnName[i].Name, columnName[i].Alise + columnName[i].Name, DataHelper.ParamChar));
            }
            return sb.ToString().TrimEnd("and".ToCharArray());
        }
        /// <summary>
        /// 用于查询数据，生成别名语句返回“col1 alise1,col2 alise2”
        /// </summary>
        /// <param name="ColumnsAsName"></param>
        /// <returns></returns>
        public string getAliseSql(Dictionary<string, string> ColumnsAsName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in ColumnsAsName.Keys)
            {
                sb.Append(String.Format("{0} {1},", item, ColumnsAsName[item]));
            }
            return sb.ToString().TrimEnd(',');
        }
        /// <summary>
        /// 用于查询数据，生成别名语句返回“col1 alise1,col2 alise2”
        /// </summary>
        /// <param name="ColumnsAsName"></param>
        /// <returns></returns>
        public string getAliseSql(List<AliseEntry> ColumnsAsName)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in ColumnsAsName)
            {
                sb.Append(String.Format("{0} {1},", item.Name, item.Alise));
            }
            return sb.ToString().TrimEnd(',');
        }
        /// <summary>
        /// 用于查询数据，生成别名语句返回“tab1.col1 alise1,tab2.col2 alise2”
        /// </summary>
        /// <param name="ColumnsAsName"></param>
        /// <returns></returns>
        public string getAliseSql(string[] tableNameAlise, Dictionary<string, string> ColumnsAsName)
        {
            if (tableNameAlise.Length != ColumnsAsName.Count) throw new LengthExcetpion("tableNameAlise长度和ColumnsAsName长度不相同，他俩必须一致");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ColumnsAsName.Keys.Count; i++)
            {
                sb.Append(String.Format("{0}.{1} {2},", tableNameAlise[i], ColumnsAsName.Keys.ElementAt(i), ColumnsAsName[ColumnsAsName.Keys.ElementAt(i)]));
            }
            return sb.ToString().TrimEnd(',');
        }
        /// <summary>
        /// 用于查询数据，生成别名语句返回“tab1.col1 alise1,tab2.col2 alise2”
        /// </summary>
        /// <param name="ColumnsAsName"></param>
        /// <returns></returns>
        public string getAliseSql(string[] tableNameAlise, List<AliseEntry> ColumnsAsName)
        {
            if (tableNameAlise.Length != ColumnsAsName.Count) throw new LengthExcetpion("tableNameAlise长度和ColumnsAsName长度不相同，他俩必须一致");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ColumnsAsName.Count; i++)
            {
                sb.Append(String.Format("{0}.{1} {2},", tableNameAlise[i], ColumnsAsName[i].Name, ColumnsAsName[i].Alise));
            }
            return sb.ToString().TrimEnd(',');
        }
        /// <summary>
        /// 生成查询数据sql 如果有where则添加where子句(不包括where单词)，没有请传null。最后生成：select XXX as YYY from TABLENAME where WHERE
        /// </summary>
        /// <returns></returns>
        public string getSelectSql(string tableName, string where, Dictionary<string, string> ColumnsAsName)
        {
            if (!String.IsNullOrEmpty(where))
            {
                return String.Format("select {0} from {1} where {2} ", getAliseSql(ColumnsAsName), tableName, where);
            }
            else return String.Format("select {0} from {1}", getAliseSql(ColumnsAsName), tableName);
        }
        /// <summary>
        /// 生成插入数据的Sql（传入表名，和列名及新值）
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string getInsertSql(string tableName, Dictionary<string, object> columnValueDics)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("insert into {0} (", tableName));
            for (int i = 0; i < columnValueDics.Keys.Count; i++)
            {
                if (i == columnValueDics.Keys.Count - 1)
                { sb.Append(string.Format(" {0}) values ", columnValueDics.Keys.ElementAt(i))); }
                else
                { sb.Append(string.Format(" {0},", columnValueDics.Keys.ElementAt(i))); }
            }
            object[] objs = new object[columnValueDics.Keys.Count];
            for (int i = 0; i < columnValueDics.Keys.Count; i++)
            {
                objs[i] = columnValueDics[columnValueDics.Keys.ElementAt(i)];
            }
            sb.Append(" ( " + getAddSql(objs) + " ) ");
            return sb.ToString();
        }
        /// <summary>
        /// 生成插入数据的Sql（传入表名，和列名） 生成：insert into TABLENAME(col1,col2,col3) values(@col1,@col2,@col3)
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string getInsertSqlParam(string tableName, List<string> columnNameList)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("insert into {0} (", tableName));
            for (int i = 0; i < columnNameList.Count; i++)
            {
                if (i == columnNameList.Count - 1)
                    sb.Append(string.Format(" {0}) values ", columnNameList.ElementAt(i)));
                else
                    sb.Append(string.Format(" {0},", columnNameList.ElementAt(i)));
            }
            sb.Append(" ( " + getAddSqlParam(columnNameList) + " ) ");
            return sb.ToString();
        }
        /// <summary>
        /// 生成更新数据的sql 如果有where则添加where子句(不包括where单词)，没有请传null。
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="ColumnsAndValue"></param>
        /// <returns></returns>
        public string getUpdateSql(string tableName, Dictionary<string, object> ColumnsAndValue, string where)
        {
            if (!String.IsNullOrEmpty(where))
            {
                return String.Format("update {0} set {1} where {2}", tableName, getEditSql(ColumnsAndValue), where);
            }
            else
            {
                return String.Format("update {0} set {1} ", tableName, getEditSql(ColumnsAndValue));
            }
        }
        /// <summary>
        /// 生成更新数据的sql 如果有where则添加where子句(不包括where单词)，没有请传null。自动生成：update TABLENAME set col1=@col1,col2=@col2 where XXX
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <param name="ColumnsAndValue"></param>
        /// <returns></returns>
        public string getUpdateSqlParam(string tableName, List<string> columnName, string where)
        {
            if (!String.IsNullOrEmpty(where))
            {
                return String.Format("update {0} set {1} where {2}", tableName, getEditSqlParam(columnName), where);
            }
            else
            {
                return String.Format("update {0} set {1} ", tableName, getEditSqlParam(columnName));
            }
        }
        /// <summary>
        /// 生成删除数据sql  如果有where则添加where子句(不包括where单词)，没有请传null。
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public string getDeleteSql(string tableName, string where)
        {
            if (!String.IsNullOrEmpty(where))
            {
                return String.Format("delete from {0} where {1} ", tableName, where);
            }
            else return String.Format("delete from {0} ", tableName);
        }
        /// <summary>
        /// 生成删除数据sql 传入条件数组， 最后生成 delete from TABLENAME where XX=XX and YY=YY
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public string getDeleteSqlParam(string tableName, Dictionary<string, object> where, string appendCustomWhere = null)
        {
            if (where != null)
            {
                return String.Format("delete from {0} where {1} {2} ", tableName, getWhereSql(where), appendCustomWhere == null ? "" : " and " + appendCustomWhere);
            }
            else return String.Format("delete from {0} {1} ", tableName, appendCustomWhere == null ? "" : "where " + appendCustomWhere);
        }
        /// <summary>
        /// 取得获取批定表的主键的例名（只获取第1个）  指定的表可以包含架构名，如：dbo.Address或guest.Address ，需要注意的是，此方法内部会调用IDataHelper对像，会使用SetDataHelperForDBType属性中的值去执行角本,如果该对像为空，将自动抛出异常。
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string getPrimeryKey(string tableName)
        {
            var colname = getPrimeryKeyList(tableName);
            if (colname.Count > 0) { return colname[0]; } else return "";
        }
        /// <summary>
        /// 取得主键列表 争对一张表有多个主键的情况。 
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<string> getPrimeryKeyList(string tableName)
        {
            return getPrimeryKeyList(this.DataHelper, tableName);
        }
        /// <summary>
        /// 取得主键列表 争对一张表有多个主键的情况。传入DataHelper对像（用于判断数据库类型），和表名。
        /// </summary>
        public List<string> getPrimeryKeyList(IDataHelper dataHelper, string tableName)
        {
            List<string> lst = new List<string>();
            if (dataHelper is Utility.SQLiteDataHelper)
            {
                var tab = dataHelper.getTable(getSQLServerPrimeryKeySql(tableName));
                if (tab != null && tab.Rows.Count > 0)
                {
                    foreach (DataRow item in tab.Rows)
                    {
                        lst.Add(item["primaryKey"] + "");
                    }
                }
                return lst;
            }
            else if (dataHelper is Utility.SQLiteDataHelper)
            {
                var tab = dataHelper.getTable("pragma  table_info('" + tableName + "')");
                if (tab != null && tab.Rows.Count > 0)
                {
                    foreach (DataRow item in tab.Rows)
                    {
                        if (item["pk"] is bool && ((bool)item["pk"]) == true || item["pk"] is long && ((long)item["pk"]) != 0)
                        { lst.Add(item["name"] + ""); }
                    }
                }
                return lst;
            }
            else if (dataHelper is Utility.MySqlDataHelper)
            {
                var tab = dataHelper.getTable("SELECT COLUMN_NAME FROM information_schema.`COLUMNS` WHERE Table_Name='" + tableName + "' AND COLUMN_Key='PRI'");
                if (tab != null && tab.Rows.Count > 0)
                {
                    foreach (DataRow item in tab.Rows)
                    {
                        lst.Add(item["COLUMN_NAME"] + "");
                    }
                }
                return lst;
            }
            else if (dataHelper is Utility.PostgreSqlDataHelper)
            {
                var sql = "select pg_constraint.conname as pk_name,pg_attribute.attname as colname,pg_type.typname as typename from pg_constraint  inner join pg_class on pg_constraint.conrelid = pg_class.oid inner join pg_attribute on pg_attribute.attrelid = pg_class.oid and  pg_attribute.attnum = pg_constraint.conkey[1] inner join pg_type on pg_type.oid = pg_attribute.atttypid where pg_class.relname = '" + tableName + "' and pg_constraint.contype='p'";
                var tab = dataHelper.getTable(sql);
                if (tab != null && tab.Rows.Count > 0)
                {
                    foreach (DataRow item in tab.Rows)
                    {
                        lst.Add(item["colname"] + "");
                    }
                }
                return lst;
            }
            else
            {
                throw new Exception("暂不支持此类数据库调用getPrimeryKeyList方法:" + dataHelper.GetType().FullName);
            }
        }

        /// <summary>
        /// 取得获取SQLServer数据库中获取主键的SQL原角本
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string getSQLServerPrimeryKeySql(string tableName)
        {
            if (tableName.Contains("."))
            {
                //如果此表包含架构名
                return String.Format("select primaryKey=COLUMN_NAME from INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE CONSTRAINT_NAME=(select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE='PRIMARY KEY' AND TABLE_NAME='{0}' and TABLE_SCHEMA='{1}')", tableName.Split('.')[1], tableName.Split('.')[0]);
            }
            else
            {   //如果不包含架构名
                return string.Format("select primaryKey=COLUMN_NAME from INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE WHERE CONSTRAINT_NAME=(select CONSTRAINT_NAME from INFORMATION_SCHEMA.TABLE_CONSTRAINTS WHERE CONSTRAINT_TYPE='PRIMARY KEY' AND TABLE_NAME='{0}')", tableName);
            }
        }
        /// <summary>
        /// 生成多表联合查询语句,需入多个表的列名和查询后要使用的别名，注意，Where子句不用写“where”单词
        /// 返回：select 别名1.Column1 列1,别名1.Column2 列2, 别名2.Column1 列3 from table1 别名1,table2 别名2 where 别名1.列1=别名2.列1 
        /// </summary>
        /// <returns></returns>
        public string getJoinQuery(AliseEntry[] tableName, Dictionary<string, string>[] TableColumnWithAlise, string customWhere)
        {
            if (tableName.Length != TableColumnWithAlise.Length) { throw new LengthExcetpion("传入的tableName和TableColumnWithAlise的长度不相同，这里必须相同！"); }
            StringBuilder sb = new StringBuilder();
            StringBuilder tableNameString = new StringBuilder();
            for (int i = 0; i < tableName.Length; i++)
            {
                tableNameString.Append(String.Format(" {0} {1},", tableName[i].Name, tableName[i].Alise));
                List<string> tableNameAlise = new List<string>();
                for (int j = 0; j < TableColumnWithAlise[i].Count; j++)
                {
                    tableNameAlise.Add(tableName[i].Alise);
                }
                sb.Append(getAliseSql(tableNameAlise.ToArray(), TableColumnWithAlise[i]));
                sb.Append(',');
            }
            if (customWhere == null)
                return String.Format("select {0} from {1} ", sb.ToString().TrimEnd(','), tableNameString.ToString().TrimEnd(','));
            else
                return String.Format("select {0} from {1} where {2}", sb.ToString().TrimEnd(','), tableNameString.ToString().TrimEnd(','), customWhere);
        }
        /// <summary>
        /// 生成多表联合查询语句,需入多个表的列名和查询后要使用的别名，注意，Where子句不用写“where”单词，还可以添加自定义where子句
        /// 返回：select 别名1.Column1 列1,别名1.Column2 列2, 别名2.Column1 列3 from table1 别名1,table2 别名2 where 别名1.列1=@别名1列1 and 别名2.列1=@别名2列1  
        /// </summary>
        public string getJoinQueryParam(AliseEntry[] tableName, Dictionary<string, string>[] TableColumnWithAlise, List<AliseEntry> paramWhere, string appendCustomWhere = null)
        {
            if (appendCustomWhere == null)
                return getJoinQuery(tableName, TableColumnWithAlise, getWhereSqlParam(paramWhere));
            else
                return getJoinQuery(tableName, TableColumnWithAlise, getWhereSqlParam(paramWhere) + " and " + appendCustomWhere);
        }
        /// <summary>
        /// 取得最近一次插入记录的主键标记。自动调用SetDataHelperForDBType的数据类型。
        /// </summary>
        /// <returns></returns>
        public string getLastInsertRowSql()
        {
            return getLastInsertRowSql(this.DataHelper);
        }
        /// <summary>
        /// 取得最近一次插入记录的主键标记。传入数据库类型IDataHelper对像。
        /// </summary>
        /// <returns></returns>
        public string getLastInsertRowSql(IDataHelper dataHelper)
        {
            if (dataHelper == null || dataHelper is Utility.SQLDataHelper)
            {
                return "select @@IDENTITY ;";
            }
            else if (dataHelper is Utility.SQLiteDataHelper)
            {
                return "select last_insert_rowid() ;";
            }
            else if (dataHelper is Utility.MySqlDataHelper)
            {
                return "select LAST_INSERT_ID() ;";
            }
            else if (dataHelper is Utility.PostgreSqlDataHelper)
            {
                return "select lastval(); ";
            }
            else
            {
                throw new Exception("暂不支持此类数据库调用getLastInsertRowSql方法:" + dataHelper.GetType().FullName);
            }
        }
        /// <summary>
        /// 传入数据库类型（IDataHelper对像），和源SQL句。将被包裹成如:select top 1 from (源Select的SQL语句) tempDataTable
        /// </summary>
        public string getTopOrLimitRecord(IDataHelper dataHelper, string sourceSql, int topCount)
        {
            if (dataHelper == null || dataHelper is Utility.SQLDataHelper)
            {
                var sql = "select top " + topCount + " * from (" + sourceSql + ")  tmp_" + Guid.NewGuid().ToString("N").Substring(0, 8);
                return sql;
            }
            else if (dataHelper is Utility.SQLiteDataHelper || dataHelper is Utility.MySqlDataHelper || dataHelper is Utility.PostgreSqlDataHelper)
            {
                var sql = "select * from (" + sourceSql + ") tmp_" + Guid.NewGuid().ToString("N").Substring(0, 8) + "  limit " + topCount;
                return sql;
            }
            else
            {
                throw new Exception("暂不支持此类数据库调用getTopOrLimitRecord方法:" + dataHelper.GetType().FullName);
            }
        }
        /// <summary>
        /// 传入源SQL句。将被包裹成如:select top 1 from (源Select的SQL语句) tempDataTable
        /// </summary>
        /// <param name="sourceSql"></param>
        /// <returns></returns>
        public string getTopOrLimitRecord(string sourceSql, int topCount)
        {
            return getTopOrLimitRecord(this.DataHelper, sourceSql, topCount);
        }
        /// <summary>
        /// 获取表结构
        /// </summary>
        /// <param name="dataHelper"></param>
        /// <param name="tableName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public TableSchema getTableSchema(IDataHelper dataHelper, string tableName, string schemaName = null)
        {
            TableSchema ts = new TableSchema();
            ts.TableName = tableName;
            ts.SchemaName = schemaName;

            if (dataHelper == null || dataHelper is Utility.SQLDataHelper)
            {
                if (string.IsNullOrWhiteSpace(schemaName)) { schemaName = "dbo"; }
                #region
                var sql = string.Format(@"SELECT sch.schemaName 架构名, CASE WHEN col.colorder = 1 THEN obj.name
                  ELSE ''
             END AS 表名,
        col.colorder AS 序号 ,
        col.name AS 列名 ,
        ISNULL(ep.[value], '') AS 列说明,
        t.name AS 数据类型 ,
        col.length AS 长度 ,
        ISNULL(COLUMNPROPERTY(col.id, col.name, 'Scale'), 0) AS 小数位数,
        CASE WHEN COLUMNPROPERTY(col.id, col.name, 'IsIdentity') = 1 THEN '1'
             ELSE ''
        END AS 标识 ,
        CASE WHEN EXISTS(SELECT   1
                           FROM     dbo.sysindexes si
                                    INNER JOIN dbo.sysindexkeys sik ON si.id = sik.id
                                                              AND si.indid = sik.indid
                                    INNER JOIN dbo.syscolumns sc ON sc.id = sik.id
                                                              AND sc.colid = sik.colid
                                    INNER JOIN dbo.sysobjects so ON so.name = si.name
                                                              AND so.xtype = 'PK'
                           WHERE    sc.id = col.id
                                    AND sc.colid = col.colid) THEN '1'
             ELSE ''
        END AS 主键 ,
        CASE WHEN col.isnullable = 1 THEN '1'
             ELSE ''
        END AS 允许空 ,
        ISNULL(comm.text, '') AS 默认值
FROM dbo.syscolumns col
        LEFT JOIN dbo.systypes t ON col.xtype = t.xusertype
        inner JOIN dbo.sysobjects obj ON col.id = obj.id
                                         AND obj.xtype = 'U'
                                         AND obj.status >= 0
        LEFT JOIN dbo.syscomments comm ON col.cdefault = comm.id
        LEFT JOIN sys.extended_properties ep ON col.id = ep.major_id
                                                      AND col.colid = ep.minor_id
                                                      AND ep.name = 'MS_Description'
        LEFT JOIN sys.extended_properties epTwo ON obj.id = epTwo.major_id
                                                         AND epTwo.minor_id = 0
                                                         AND epTwo.name = 'MS_Description'
		Left join (select t.object_id as objId,s.[name] as schemaName from sys.tables t,sys.schemas s where t.schema_id = s.schema_id ) sch on sch.objId= obj.id
WHERE obj.name = '{0}'--表名
AND sch.schemaName='{1}' --架构名
ORDER BY col.colorder; ", tableName, schemaName);
                #endregion
                var sqlserverStructTab = dataHelper.getTable(sql);
                foreach (DataRow dr in sqlserverStructTab.Rows)
                {
                    var colName = dr["列名"] + "";
                    var typeName = (dr["数据类型"] + "").ToLower();
                    var typeLen = 0;
                    int.TryParse(dr["长度"] + "", out typeLen);
                    var dotLen = 0;
                    int.TryParse(dr["小数位数"] + "", out dotLen);
                    var isPrimaryKey = (dr["主键"] + "").Trim() == "1";
                    var canNull = (dr["允许空"] + "").Trim() == "1";
                    var canAutoIncrement = (dr["标识"] + "").Trim() == "1";
                    var defaultValue = string.IsNullOrWhiteSpace(dr["默认值"] + "") ? "" : ("DEFAULT " + (dr["默认值"] + ""));
                    var colDescription = string.IsNullOrWhiteSpace(dr["列说明"] + "") ? "" : ("/*" + dr["列说明"] + "*/");
                    ts.Columns.Add(new ColumnSchema { ColumnName = colName, DataType = typeName, Len = typeLen, DotLen = dotLen, IsPrimaryKey = isPrimaryKey, IsNullable = canNull, IsIdentity = canAutoIncrement, DefaultValue = defaultValue, Description = colDescription });
                }
            }
            else if (dataHelper is Utility.SQLiteDataHelper)
            {
                var sqliteStructTab = dataHelper.getTable("PRAGMA table_info([" + tableName + "])");
                foreach (DataRow dr in sqliteStructTab.Rows)
                {
                    var colName = dr["name"] + "";
                    var typeName = (dr["type"] + "").ToLower();
                    var isPrimaryKey = (dr["pk"] + "").Trim() == "1";
                    var canNull = (dr["notnull"] + "").Trim() != "1";
                    var defaultValue = string.IsNullOrWhiteSpace(dr["dflt_value"] + "") ? "" : ("DEFAULT (" + (dr["dflt_value"] + ")"));
                    ts.Columns.Add(new ColumnSchema { ColumnName = colName, DataType = typeName, IsPrimaryKey = isPrimaryKey, IsNullable = canNull, DefaultValue = defaultValue });
                }
            }
            else if (dataHelper is Utility.MySqlDataHelper)
            {
                var sql = string.Format("select table_schema,table_name,column_name,column_default,is_nullable,data_type,character_maximum_length,column_type,column_key,extra from information_schema.columns  where table_name='{0}' {1}", tableName, (string.IsNullOrWhiteSpace(schemaName) ? "" : ("and table_schema='" + schemaName + "'")));
                var sqliteStructTab = dataHelper.getTable(sql);
                foreach (DataRow dr in sqliteStructTab.Rows)
                {
                    var colName = dr["column_name"] + "";
                    var typeName = (dr["data_type"] + "").ToLower();
                    var typeLen = 0;
                    int.TryParse(dr["character_maximum_length"] + "", out typeLen);
                    var isPrimaryKey = (dr["column_key"] + "").Trim().ToLower() == "PRI".ToLower();
                    var canNull = (dr["is_nullable"] + "").Trim().ToLower() == "YES".ToLower();
                    var auto_increment = (dr["extra"] + "").Trim().ToLower().Contains("auto_increment".ToLower());
                    var defaultValue = string.IsNullOrWhiteSpace(dr["column_default"] + "") ? "" : ("DEFAULT '" + (dr["column_default"] + "'"));
                    ts.Columns.Add(new ColumnSchema { ColumnName = colName, DataType = typeName, IsPrimaryKey = isPrimaryKey, IsNullable = canNull, DefaultValue = defaultValue, IsIdentity = auto_increment, Len = typeLen });
                }
            }
            else if (dataHelper is Utility.PostgreSqlDataHelper)
            {
                #region
                var sql = string.Format(@"SELECT attname,typname,adsrc,atttypmod,attlen,attnotnull
                  FROM
                   pg_attribute
                   INNER JOIN pg_class  ON pg_attribute.attrelid = pg_class.oid
                   INNER JOIN pg_type   ON pg_attribute.atttypid = pg_type.oid
                   LEFT OUTER JOIN pg_attrdef ON pg_attrdef.adrelid = pg_class.oid AND pg_attrdef.adnum = pg_attribute.attnum
                   LEFT OUTER JOIN pg_description ON pg_description.objoid = pg_class.oid AND pg_description.objsubid = pg_attribute.attnum
                  WHERE  pg_attribute.attnum > 0  AND attisdropped <> 't'  AND pg_class.relname= '{0}'  ORDER BY pg_attribute.attnum ;", tableName);
                var postgresqlStructTab = dataHelper.getTable(sql);
                foreach (DataRow dr in postgresqlStructTab.Rows)
                {
                    var colName = dr["attname"] + "";
                    var typeName = (dr["typname"] + "").ToLower();
                    var typeLen = 0;
                    int.TryParse(dr["atttypmod"] + "", out typeLen);
                    var canNull = (dr["attnotnull"] + "").Trim().ToLower() != "1".ToLower();
                    var defaultValue = string.IsNullOrWhiteSpace(dr["adsrc"] + "") ? "" : ("DEFAULT " + (dr["adsrc"]));
                    ts.Columns.Add(new ColumnSchema { ColumnName = colName, DataType = typeName, IsNullable = canNull, DefaultValue = defaultValue, Len = typeLen });
                }
                sql = string.Format(@"select 
                pg_constraint.conname as pk_name,
                pg_attribute.attname as colname,
                pg_type.typname as typename 
                from 
                pg_constraint  
                inner join pg_class on pg_constraint.conrelid = pg_class.oid 
                inner join pg_attribute on pg_attribute.attrelid = pg_class.oid  and  pg_attribute.attnum = any(pg_constraint.conkey)
                inner join pg_type on pg_type.oid = pg_attribute.atttypid
                where pg_class.relname = '{0}'
                and pg_constraint.contype='p'
                and pg_table_is_visible(pg_class.oid)", tableName);
                var postgresqlPrimarykeyTab = dataHelper.getTable(sql);
                foreach (DataRow dr in postgresqlPrimarykeyTab.Rows)
                {
                    var colName = (dr["colname"] + "").ToLower();
                    var typeName = (dr["typename"] + "").ToLower();
                    var find = ts.Columns.FirstOrDefault(cs => (cs.ColumnName + "").ToLower() == colName && (cs.DataType + "").ToLower() == typeName);
                    if (find != null) find.IsPrimaryKey = true;
                }
                #endregion
            }
            else
            {
                throw new NotSupportedException("getTableSchemaSql方法当前不尚未支持：" + dataHelper.GetType().FullName);
            }
            return ts;
        }
        /// <summary>
        /// 取得表结构
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public TableSchema getTableSchema(string tableName, string schemaName = null)
        {
            return this.getTableSchema(this.DataHelper, tableName, schemaName);
        }
        /// <summary>
        /// 使用特殊字符包围，一般用于处理特殊名称的表名或列名。
        /// </summary>
        /// <returns></returns>
        public string safeName(IDataHelper dataHelper, string tableNameOrColumnName)
        {
            if (dataHelper is Utility.SQLiteDataHelper)
            { return string.Format("\"{0}\"", tableNameOrColumnName); }
            if (dataHelper is Utility.SQLDataHelper)
            { return string.Format("[{0}]", tableNameOrColumnName); }
            if (dataHelper is Utility.MySqlDataHelper)
            { return string.Format("`{0}`", tableNameOrColumnName); }
            if (dataHelper is Utility.PostgreSqlDataHelper)
            { return string.Format("\"{0}\"", tableNameOrColumnName); }
            return tableNameOrColumnName;
        }
        /// <summary>
        /// 使用特殊字符包围，一般用于处理特殊名称的表名或列名。
        /// </summary>
        /// <returns></returns>
        public string safeName(string tableNameOrColumnName)
        {
            return safeName(this.DataHelper, tableNameOrColumnName);
        }
        /// <summary>
        /// 取得C#类型和数据库的映射关系。
        /// </summary>
        /// <returns></returns>
        public string GetTypeToDbTypeMapping(Type csType, int len)
        {
            return GetTypeToDbTypeMapping(this.DataHelper, csType, len);
        }
        /// <summary>
        /// 取得C#类型和数据库的映射关系。
        /// </summary>
        public string GetTypeToDbTypeMapping(IDataHelper dataHelper, Type csType, int len)
        {
            Dictionary<string, string> instence = new Dictionary<string, string>();
            if (dataHelper == null || dataHelper is Utility.SQLDataHelper)
            {
                instence.Clear();
                instence.Add(typeof(int).FullName, "int");
                instence.Add(typeof(bool).FullName, "bit");
                instence.Add(typeof(char).FullName, "nvarchar(1)");
                instence.Add(typeof(byte).FullName, "tinyint");
                instence.Add(typeof(short).FullName, "smallint");
                instence.Add(typeof(long).FullName, "bigint");
                instence.Add(typeof(float).FullName, "real");
                instence.Add(typeof(double).FullName, "money");
                instence.Add(typeof(decimal).FullName, "money");
                instence.Add(typeof(string).FullName, "nvarchar(max)");
                instence.Add(typeof(byte[]).FullName, "image");
                instence.Add(typeof(DateTime).FullName, "datetime");
                instence.Add(typeof(Guid).FullName, "uniqueidentifier");

                instence.Add(typeof(int?).FullName, "int");
                instence.Add(typeof(bool?).FullName, "bit");
                instence.Add(typeof(char?).FullName, "nvarchar(1)");
                instence.Add(typeof(byte?).FullName, "tinyint");
                instence.Add(typeof(short?).FullName, "smallint");
                instence.Add(typeof(long?).FullName, "bigint");
                instence.Add(typeof(float?).FullName, "real");
                instence.Add(typeof(double?).FullName, "money");
                instence.Add(typeof(decimal?).FullName, "money");
                instence.Add(typeof(DateTime?).FullName, "datetime");
                instence.Add(typeof(Guid?).FullName, "uniqueidentifier");

            }
            else if (dataHelper is Utility.SQLiteDataHelper)
            {
                instence.Clear();
                instence.Add(typeof(int).FullName, "integer");
                instence.Add(typeof(bool).FullName, "integer");
                instence.Add(typeof(char).FullName, "text");
                instence.Add(typeof(byte).FullName, "integer");
                instence.Add(typeof(short).FullName, "integer");
                instence.Add(typeof(long).FullName, "integer");
                instence.Add(typeof(float).FullName, "real");
                instence.Add(typeof(double).FullName, "real");
                instence.Add(typeof(decimal).FullName, "real");
                instence.Add(typeof(string).FullName, "text");
                instence.Add(typeof(byte[]).FullName, "blob(2147483647)");
                instence.Add(typeof(DateTime).FullName, "datetime");
                instence.Add(typeof(Guid).FullName, "guid");

                instence.Add(typeof(int?).FullName, "integer");
                instence.Add(typeof(bool?).FullName, "integer");
                instence.Add(typeof(char?).FullName, "text");
                instence.Add(typeof(byte?).FullName, "integer");
                instence.Add(typeof(short?).FullName, "integer");
                instence.Add(typeof(long?).FullName, "integer");
                instence.Add(typeof(float?).FullName, "real");
                instence.Add(typeof(double?).FullName, "real");
                instence.Add(typeof(decimal?).FullName, "real");
                instence.Add(typeof(DateTime?).FullName, "datetime");
                instence.Add(typeof(Guid?).FullName, "guid");

            }
            else if (dataHelper is Utility.MySqlDataHelper)
            {
                instence.Clear();
                instence.Add(typeof(int).FullName, "int");
                instence.Add(typeof(bool).FullName, "bit");
                instence.Add(typeof(char).FullName, "varchar(2)");
                instence.Add(typeof(byte).FullName, "tinyint");
                instence.Add(typeof(short).FullName, "smallint");
                instence.Add(typeof(long).FullName, "bigint");
                instence.Add(typeof(float).FullName, "float");
                instence.Add(typeof(double).FullName, "decimal");
                instence.Add(typeof(decimal).FullName, "decimal");
                instence.Add(typeof(string).FullName, "mediumtext");
                instence.Add(typeof(byte[]).FullName, "longblob");
                instence.Add(typeof(DateTime).FullName, "datetime");
                instence.Add(typeof(Guid).FullName, "varchar(36)");

                instence.Add(typeof(int?).FullName, "int");
                instence.Add(typeof(bool?).FullName, "bit");
                instence.Add(typeof(char?).FullName, "varchar(2)");
                instence.Add(typeof(byte?).FullName, "tinyint");
                instence.Add(typeof(short?).FullName, "smallint");
                instence.Add(typeof(long?).FullName, "bigint");
                instence.Add(typeof(float?).FullName, "float");
                instence.Add(typeof(double?).FullName, "double");
                instence.Add(typeof(decimal?).FullName, "double");
                instence.Add(typeof(DateTime?).FullName, "datetime");
                instence.Add(typeof(Guid?).FullName, "varchar(36)");
            }
            else if (dataHelper is Utility.PostgreSqlDataHelper)
            {
                instence.Clear();
                instence.Add(typeof(int).FullName, "integer");
                instence.Add(typeof(bool).FullName, "boolean");
                instence.Add(typeof(char).FullName, "varchar(2)");
                instence.Add(typeof(byte).FullName, "smallint");
                instence.Add(typeof(short).FullName, "smallint");
                instence.Add(typeof(long).FullName, "bigint");
                instence.Add(typeof(float).FullName, "real");
                instence.Add(typeof(double).FullName, "money");
                instence.Add(typeof(decimal).FullName, "money");
                instence.Add(typeof(string).FullName, "text");
                instence.Add(typeof(byte[]).FullName, "bytea");
                instence.Add(typeof(DateTime).FullName, "timestamp");
                instence.Add(typeof(Guid).FullName, "uuid");

                instence.Add(typeof(int?).FullName, "integer");
                instence.Add(typeof(bool?).FullName, "boolean");
                instence.Add(typeof(char?).FullName, "varchar(2)");
                instence.Add(typeof(byte?).FullName, "smallint");
                instence.Add(typeof(short?).FullName, "smallint");
                instence.Add(typeof(long?).FullName, "bigint");
                instence.Add(typeof(float?).FullName, "real");
                instence.Add(typeof(double?).FullName, "money");
                instence.Add(typeof(decimal?).FullName, "money");
                instence.Add(typeof(DateTime?).FullName, "timestamp");
                instence.Add(typeof(Guid?).FullName, "uuid");

            }

            //返回相应类型。
            foreach (var key in instence.Keys)
            {
                if (key == csType.FullName)
                {
                    return instence[key];
                }
            }
            return "";
        }
    }
}
