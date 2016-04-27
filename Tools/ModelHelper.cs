using Cocon90.Lib.Dal.Error;
using Cocon90.Lib.Dal.Rule;
using Cocon90.Lib.Dal.Rule.Attribute;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cocon90.Lib.Dal.Tools
{
    /// <summary>
    /// 实体类辅助类
    /// </summary>
    /// <typeparam name="T">此类型必须要有一个无参的构造函数，且实现IModel</typeparam>
    public class ModelHelper<T> where T : class,Rule.IModel, new()
    {
        /// <summary>
        /// 构建一个ModelHelper ，传入Config文件中的连接字符串的Name，默认使用“ConnectionString”
        /// </summary>
        public ModelHelper(string connectionStringName = "ConnectionString")
        {
            this.DataHelper = DataHelperFactory.CreateInstence(connectionStringName);
            this.SqlHelper = new SqlHelper(this.DataHelper);
        }
        /// <summary>
        /// 构建一个ModelHelper 
        /// </summary>
        public ModelHelper(string providerName, string connectionString)
        {
            this.DataHelper = DataHelperFactory.CreateInstence(providerName, connectionString);
            this.SqlHelper = new SqlHelper(this.DataHelper);
        }
        /// <summary>
        /// 构建一个ModelHelper  
        /// </summary>
        public ModelHelper(IDataHelper dataHelper)
        {
            this.DataHelper = dataHelper;
            this.SqlHelper = new SqlHelper(this.DataHelper);
        }
        /// <summary>
        /// 获到当前的SqlHelper辅助类实例
        /// </summary>
        public SqlHelper SqlHelper { get; set; }
        /// <summary>
        /// 获取一个DataHelper实例（由本类的构造函数指定配置的连接语句名称。）
        /// </summary>
        public IDataHelper DataHelper { get; private set; }
        /// <summary>
        /// DataHelper属性的简写。  用于获取一个DataHelper实例（由本类的构造函数指定配置的连接语句名称。）
        /// </summary>
        public IDataHelper dh { get { return this.DataHelper; } }
        /// <summary>
        /// SqlHelper属性的简写
        /// </summary>
        public SqlHelper sh { get { return this.SqlHelper; } }

        /// <summary>
        /// 取得所有记录List，传入自定义条件。如果条件不存在，请传入NULL。
        /// </summary>
        public List<T> GetListByCustomWhere(string customWhere, params Parameter[] paramKeyValue)
        {
            if (string.IsNullOrWhiteSpace(customWhere)) { return GetList("select * from " + GetTableName()); }
            else { return GetList("select * from " + GetTableName() + " where " + customWhere, paramKeyValue); }
        }
        /// <summary>
        /// 取得所有记录List
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public List<T> GetList()
        {
            return GetList("select * from " + GetTableName());
        }

        /// <summary>
        /// 由传入的Sql语句来查询出对应的List
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public List<T> GetList(string tsqlParamed, params Parameter[] paramKeyValue)
        {
            return GetList(dh.getTable(tsqlParamed, paramKeyValue));
        }
        /// <summary>
        /// 判断查询结果是否有行记录。
        /// </summary>
        /// <param name="customWhere">自定义Where子句，不需要加“where”单词。</param>
        /// <param name="paramKeyValue">参数列表</param>
        /// <returns></returns>
        public bool ExistByCustomWhere(string customWhere, params Parameter[] paramKeyValue)
        {
            var one = this.GetOneByAttribute(customWhere, paramKeyValue);
            if (one != null) { return true; }
            else return false;
        }
        /// <summary>
        /// 判断查询结果是否有行记录。
        /// </summary>
        /// <param name="tsqlParamed">Sql语句</param>
        /// <param name="paramKeyValue">参数列表</param>
        /// <returns></returns>
        public bool Exist(string tsqlParamed, params Parameter[] paramKeyValue)
        {
            var tab = dh.getTable(tsqlParamed, paramKeyValue);
            if (tab != null && tab.Rows.Count > 0) { return true; }
            else return false;
        }
        /// <summary>
        /// 由DataTable返回此类型模型集合
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public List<T> GetList(System.Data.DataTable table)
        {
            DataModel<T> dm = new DataModel<T>();
            return dm.GetList(table);
        }
        /// <summary>
        /// 通过实体的扩展属性标记取得实体集合。按主键升序排序，如果查询无结果，则返回空List。另外，customWhere中不必输入"Where";
        /// </summary>
        public List<T> GetListByAttribute(string customWhere, params Parameter[] paramKeyValue)
        {
            var primeryKey = GetPrimeryKey();
            return this.GetListByAttribute(primeryKey, customWhere, paramKeyValue);
        }
        /// <summary>
        /// 通过实体的扩展属性标记取得实体集合。需要要传入排序字段，返回所有记录。如果查询无结果，则返回空List。另外，customWhere中不必输入"Where";
        /// </summary>
        public List<T> GetListByAttribute(string orderColumnName, string customWhere, params Parameter[] paramKeyValue)
        {
            return GetListByAttribute(orderColumnName, true, 1, int.MaxValue, customWhere, paramKeyValue);
        }
        /// <summary>
        /// 通过实体的扩展属性标记取得实体集合。需要要传入排序字段，和分页信息（pageNum>=1），如果查询无结果，则返回空List。另外，customWhere中不必输入"Where";
        /// </summary>
        public List<T> GetListByAttribute(string orderColumnName, bool isAsc, int pageNum, int pageSize, string customWhere, params Parameter[] paramKeyValue)
        {
            var pm = this.GetPagedInfoViewUseAttrbute(orderColumnName, isAsc, pageNum, pageSize, customWhere, paramKeyValue);
            if (pm != null && pm.List != null) { return pm.List; }
            return new List<T>();
        }
        /// <summary>
        /// 取得单个模型，如果无记录，则反回null
        /// </summary>
        /// <param name="tsqlParamed"></param>
        /// <param name="paramKeyValue"></param>
        /// <returns></returns>
        public T GetOne(string tsqlParamed, params Parameter[] paramKeyValue)
        {
            var lst = GetList(tsqlParamed, paramKeyValue);
            if (lst == null || lst.Count <= 0) return null;
            else return lst[0];
        }

        /// <summary>
        /// 取得单个模型，如果无记录，则反回null
        /// </summary>
        /// <param name="tsqlParamed"></param>
        /// <param name="paramKeyValue"></param>
        /// <returns></returns>
        public T GetOneByPrimeryKey(object paramKeyValue)
        {
            string primeryKey = GetPrimeryKey();
            string tsqlParamed = String.Format("select * from {0} where {1}={2}{1}", GetTableName(), primeryKey, this.DataHelper.ParamChar);
            var lst = GetList(tsqlParamed, new Parameter(this.DataHelper.ParamChar + primeryKey, paramKeyValue));
            if (lst == null || lst.Count <= 0) return null;
            else return lst[0];
        }
        /// <summary>
        /// 通过实体的扩展属性标记取得单个实体。(系统自动取主键排序)如果查询无结果，则返回NULL。另外，customWhere中不必输入"Where";
        /// </summary>
        public T GetOneByAttribute(string customWhere, params Parameter[] paramKeyValue)
        {
            var primeryKey = GetPrimeryKey();
            return this.GetOneByAttribute(primeryKey, customWhere, paramKeyValue);
        }
        /// <summary>
        /// 通过实体的扩展属性标记取得单个实体。(请传入一个结果列的列名，作为排序字段) 如果查询无结果，则返回NULL。另外，customWhere中不必输入"Where";
        /// </summary>
        public T GetOneByAttribute(string orderColumnName, string customWhere, params Parameter[] paramKeyValue)
        {
            var pm = this.GetPagedInfoViewUseAttrbute(orderColumnName, true, 1, 1, customWhere, paramKeyValue);
            if (pm != null && pm.List != null && pm.List.Count > 0) { return pm.List[0]; }
            return null;
        }
        /// <summary>
        /// 取得表中第一行，作为单个模型，如果无记录，则反回null
        /// </summary>
        /// <param name="tsqlParamed"></param>
        /// <param name="paramKeyValue"></param>
        /// <returns></returns>
        public T GetOne(System.Data.DataTable table)
        {
            var lst = GetList(table);
            if (lst == null || lst.Count <= 0) return null;
            else return lst[0];
        }
        /// <summary>
        /// 取得当前Model的主键列名
        /// </summary>
        /// <returns></returns>
        public string GetPrimeryKey()
        {
            var model = (T)Activator.CreateInstance(typeof(T));

            var primeryKey = this.SqlHelper.getPrimeryKey(model.TableName);
            return primeryKey;
        }
        /// <summary>
        /// 取得指定Model类型对应的表主键列名
        /// </summary>
        /// <param name="modelType"></param>
        /// <returns></returns>
        public string GetPrimeryKey(Type modelType)
        {
            var model = (T)Activator.CreateInstance(modelType);

            var primeryKey = this.SqlHelper.getPrimeryKey(model.TableName);
            return primeryKey;
        }
        /// <summary>
        /// 取得Model类型的表名
        /// </summary>
        /// <returns></returns>
        public string GetTableName()
        {
            var model = (T)Activator.CreateInstance(typeof(T));
            return model.TableName;
        }
        /// <summary>
        /// 取得指定的Model类型的表名
        /// </summary>
        /// <returns></returns>
        public string GetTableName(Type modelType)
        {
            var model = (T)Activator.CreateInstance(modelType);
            return model.TableName;
        }
        /// <summary>
        /// 取得记录总数，可以添加自定义Where子句，不需包含where单词
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int GetRowCount(string customWhere = null)
        {
            var model = (T)Activator.CreateInstance(typeof(T));

            var primeryKey = SqlHelper.getPrimeryKey(model.TableName);
            if (primeryKey + "" == "") { primeryKey = "*"; }
            if (customWhere == null)
                return dh.getNumber(String.Format("select count({0}) from {1} ", primeryKey, model.TableName));
            else
                return dh.getNumber(String.Format("select count({0}) from {1} where {2}", primeryKey, model.TableName, customWhere));
        }
        /// <summary>
        /// 判断指定类型中是否包含指定名称的属性。属性名称区分大小写。
        /// </summary>
        public bool ContainsProperty(Type type, string PropertyName)
        {
            foreach (var item in type.GetProperties())
            {
                if (item.Name == PropertyName)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        ///将Model实体映射成List集合。可以指定是否把Model中值为NULL的属性也映射进去。
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isContainNullValue"></param>
        /// <returns></returns>
        public List<string> GetColumnNames(T model, bool isContainNullValue)
        {
            List<string> list = new List<string>();
            Type typeFromHandle = typeof(T);
            PropertyInfo[] properties = typeFromHandle.GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (model is Rule.IUseless)
                {
                    var useless = model as Rule.IUseless;
                    if (useless != null && useless.UselessProperty != null)
                    {
                        if (useless.UselessProperty.Contains(propertyInfo.Name)) continue;
                    }
                }
                if (!this.ContainsProperty(typeof(IModel), propertyInfo.Name))
                {
                    if (isContainNullValue)
                    {
                        if (propertyInfo.CanWrite)
                        {
                            list.Add(propertyInfo.Name);
                        }
                    }
                    else
                    {
                        if (propertyInfo.GetValue(model, null) != null)
                        {
                            if (propertyInfo.CanWrite)
                            {
                                list.Add(propertyInfo.Name);
                            }
                        }
                    }
                }

            }
            List<string> lst = new List<string>(list.ToArray());
            foreach (var col in list)
            {
                var prop = model.GetType().GetProperty(col);
                if (prop != null)
                {
                    var attrs = prop.GetCustomAttributes(typeof(RelationAttribute), true);
                    foreach (RelationAttribute item in attrs)
                    {
                        lst.Remove(col);
                    }
                }
            }
            return lst;
        }
        /// <summary>
        /// 添加记录（添加IModel类型实体，如果某项属性不想添加，或者使用默认值，请将该列设置为NULL或者使用默认的可空类型）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(T model)
        {
            var sql = this.GetInsertSqlByModel(model);
            return dh.execBatch(new[] { sql }, true, true) > 0;
        }
        /// <summary>
        /// 添加记录（添加IModel类型实体，如果某项属性不想添加，或者使用默认值，请将该列设置为NULL或者使用默认的可空类型） GeneratedKey返回自增长的主键值，出现异常时反回-1
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Add(T model, out long GeneratedKey)
        {
            try
            {
                List<string> columnsNames = GetColumnNames(model, false);
                var sqlParam = SqlHelper.getInsertSqlParam(model.TableName, columnsNames);//生成包含@Col1,@Col2等带参的SQL语句
                List<Parameter> dbParams = new List<Parameter>();
                foreach (var colName in columnsNames)
                {
                    dbParams.Add(new Parameter(String.Format("{1}{0}", colName, dh.ParamChar), typeof(T).GetProperty(colName).GetValue(model, null)));
                }
                try
                {
                    GeneratedKey = long.Parse("0" + dh.getString(sqlParam + ";" + SqlHelper.getLastInsertRowSql(), dbParams.ToArray()));
                }
                catch { GeneratedKey = 0; }
                return true;
            }
            catch { GeneratedKey = -1; return false; }
        }
        /// <summary>
        /// 修改记录，需传入该表主键的值（修改IModel类型实体，如果某项属性不想修改，或者使用默认值，请将该列设置为NULL或者使用默认的可空类型）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool EditByPrimeryKey(T model, object primeryKeyValue, bool isNullMeansNotChange = true)
        {
            var sql = this.GetUpdateSqlByPrimeryKey(model, primeryKeyValue, isNullMeansNotChange);
            return dh.execBatch(new[] { sql }, true, true) > 0;
        }
        /// <summary>
        /// 修改记录，需传入自定义Where子句(不包括"where单词")，若没有Where请传NULL值（修改IModel类型实体，如果某项属性不想修改，或者使用默认值，请将该列设置为NULL或者使用默认的可空类型）
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool EditByCustomWhere(T model, string customWhereStirng, bool isNullMeansNotChange = true, params Parameter[] whereParas)
        {
            var sql = this.GetUpdateSqlByCustomWhere(model, customWhereStirng, isNullMeansNotChange, whereParas);
            return dh.execBatch(new[] { sql }, true, true) > 0;
        }
        /// <summary>
        /// 删除记录，传入主键值。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteByPrimeryKey(object primeryKeyValue)
        {
            var model = (T)Activator.CreateInstance(typeof(T));

            var primeryKey = SqlHelper.getPrimeryKey(model.TableName);
            return DeleteByCustomWhere(primeryKey + "=" + dh.ParamChar + primeryKey, new Parameter(dh.ParamChar + primeryKey, primeryKeyValue));

        }
        /// <summary>
        /// 删除记录，删除IModel类型实体，除主键之处全传入null即可。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool DeleteByModel(T model)
        {

            var primeryKey = this.GetPrimeryKey(typeof(T));
            return DeleteByPrimeryKey(typeof(T).GetProperty(primeryKey).GetValue(model, null));
        }
        /// <summary>
        /// 删除记录，传入自定义Where子句（不包括where单词）
        /// </summary>
        /// <returns></returns>
        public bool DeleteByCustomWhere(string customWhereStirng, params Parameter[] whereParas)
        {
            var model = (T)Activator.CreateInstance(typeof(T));
            var sql = SqlHelper.getDeleteSql(model.TableName, customWhereStirng);
            if (whereParas != null) return dh.execNoQuery(sql, whereParas) > 0;
            return dh.execNoQuery(sql) > 0;
        }
        /// <summary>
        /// 判断两个模型的值 是否一一对应相等。
        /// </summary>
        /// <param name="oldModel"></param>
        /// <param name="newModel"></param>
        /// <returns></returns>
        public bool IsModelEqual(T oldModel, T newModel)
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            foreach (var prop in properties)
            {
                var oldPropValue = prop.GetValue(oldModel, null);
                var newPropValue = prop.GetValue(newModel, null);
                if (oldPropValue + "" != newPropValue + "")
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 取得多表联合查询的分页信息。
        /// </summary>
        /// <param name="tableNameWithAliasArray">多表的表名及别名，中间用空格隔开，如：new string[]{"studentTab st","teacherTab as tt"}，也可以直接传入“"studentTab st,teacherTab tt".Split(',')”</param>
        /// <param name="selectColumnNameWithAlias">多表的查询列，中间用空格隔开，如：new string[]{"name 姓名","age 性别","(select count(*) from XXXTab) as 总数"}，也可以直接传入“"name 姓名,age 性别,(select count(*) from XXXTab) as 总数".Split(',')”</param>
        /// <param name="orderColumnName">排序列的列名，此列名一般是存在于查询列中的一员</param>
        /// <param name="isAsc">是否升序排列，升序则为true,降序则为false</param>
        /// <param name="pageNumber">返回第几页</param>
        /// <param name="pageSize">每页记录总数</param>
        /// <param name="customWhere">自定义的Where子句，如果没有，请传入Null或空字符串</param>
        /// <param name="paramKeyValue">依次传入自定义Where子句中的参数。参数中请使用@做为参数，如： new Parameter("@name","宋兴柱")</param>
        /// <returns>返回当前页的DataTable以及实际数据的总数</returns>
        public Rule.PagedModel GetPagedInfo(string[] tableNameWithAliasArray, string[] selectColumnNameWithAlias, string orderColumnName, bool isAsc, int pageNumber, int pageSize, string customWhere, params Parameter[] paramKeyValue)
        {
            Rule.PagedModel pm = new Rule.PagedModel();
            if (tableNameWithAliasArray == null || tableNameWithAliasArray.Length == 0) return pm;
            var baseSql = String.Join(",", tableNameWithAliasArray) + " " + ((customWhere + "").Trim() == "" ? "" : " where " + customWhere);
            var selectSql = "select " + ((selectColumnNameWithAlias == null || selectColumnNameWithAlias.Length == 0) ? "*" : string.Join(",", selectColumnNameWithAlias)) + " from " + baseSql; ;
            var pagerSql = "select count(*) from " + baseSql;
            var sql = SqlHelper.getPagedSql(selectSql, orderColumnName, isAsc, pageNumber, pageSize);
            pm.Table = dh.getTable(sql, paramKeyValue);
            pm.Total = dh.getNumber(pagerSql, paramKeyValue); ;
            return pm;
        }
        /// <summary>
        /// 分页取得实体所对应表中的数据集合和总记录数，传入排序方式，是否升序，页数，每页大小，自定义Where子句（不用加关键字'where'，没有请传NULL或""），Where子句中的参数列表。
        /// </summary>
        public Rule.PagedModel<T> GetPagedInfo(string orderColumnName, bool isAsc, int pageNumber, int pageSize, string customWhere, params Parameter[] paramKeyValue)
        {
            Rule.PagedModel<T> pm = new Rule.PagedModel<T>();
            if ((orderColumnName + "").Trim() == null) return pm;
            var tObject = (T)Activator.CreateInstance(typeof(T));
            var baseSql = tObject.TableName + " " + ((customWhere + "").Trim() == "" ? "" : " where " + customWhere);
            var selectSql = "select * from " + baseSql; ;
            var pagerSql = "select count(*) from " + baseSql;
            var sql = SqlHelper.getPagedSql(selectSql, orderColumnName, isAsc, pageNumber, pageSize);
            pm.List = GetList(sql, paramKeyValue);
            pm.Total = dh.getNumber(pagerSql, paramKeyValue); ;
            return pm;
        }

        /// <summary>
        /// 取得多表联合查询的分页信息。泛型 T 可以使用实体对像的子类。
        /// </summary>
        /// <param name="tableNameWithAliasArray">多表的表名及别名，中间用空格隔开，如：new string[]{"studentTab st","teacherTab as tt"}，也可以直接传入“"studentTab st,teacherTab tt".Split(',')”</param>
        /// <param name="selectColumnNameWithAlias">多表的查询列，中间用空格隔开，如：new string[]{"name 姓名","age 性别","(select count(*) from XXXTab) as 总数"}，也可以直接传入“"name 姓名,age 性别,(select count(*) from XXXTab) as 总数".Split(',')”</param>
        /// <param name="orderColumnName">排序列的列名，此列名一般是存在于查询列中的一员</param>
        /// <param name="isAsc">是否升序排列，升序则为true,降序则为false</param>
        /// <param name="pageNumber">返回第几页</param>
        /// <param name="pageSize">每页记录总数</param>
        /// <param name="customWhere">自定义的Where子句，如果没有，请传入Null或空字符串</param>
        /// <param name="paramKeyValue">依次传入自定义Where子句中的参数。参数中请使用@做为参数，如：new Parameter("@name","宋兴柱")</param>
        /// <returns>返回当前页的List以及实际数据的总数</returns>
        public PagedModel<T> GetPagedInfoView(string[] tableNameWithAliasArray, string[] selectColumnNameWithAlias, string orderColumnName, bool isAsc, int pageNumber, int pageSize, string customWhere, params Parameter[] paramKeyValue)
        {
            var pm = GetPagedInfo(tableNameWithAliasArray, selectColumnNameWithAlias, orderColumnName, isAsc, pageNumber, pageSize, customWhere, paramKeyValue);
            DataModel<T> dm = new DataModel<T>();
            PagedModel<T> pmm = new PagedModel<T>();
            pmm.List = dm.GetList(pm.Table);
            pmm.Total = pm.Total;
            return pmm;
        }
        internal PagedModel<T> GetPagedInfoView(IList<ModelPoster> modelPosterList, string[] tableNameWithAliasArray, string[] selectColumnNameWithAlias, string orderColumnName, bool isAsc, int pageNumber, int pageSize, string customWhere, params Parameter[] paramKeyValue)
        {
            var pm = GetPagedInfo(tableNameWithAliasArray, selectColumnNameWithAlias, orderColumnName, isAsc, pageNumber, pageSize, customWhere, paramKeyValue);
            DataModel<T> dm = new DataModel<T>();
            PagedModel<T> pmm = new PagedModel<T>();
            pmm.List = dm.GetList(pm.Table);
            foreach (var poster in modelPosterList)
            {
                foreach (var model in pmm.List)
                {
                    var prop = model.GetType().GetProperty(poster.PropertyName);
                    if (prop != null && prop.CanWrite)
                    {
                        var modelProp = model.GetType().GetProperty(poster.ModelFromAttribute.CurrentPropertyAsEqual_right);
                        if (modelProp == null) { throw new ModelAttrbuteException("实体标记属性参数：currentPropertyAsEqual_right的值不正确，应输入当前类中某一个属性的名称。(当前传入的值是：" + poster.ModelFromAttribute.CurrentPropertyAsEqual_right + "，而在实体中未找到此属性)"); }
                        var rightValue = modelProp.GetValue(model, null);
                        var value = dh.getTable(poster.ModelFromAttribute.getSelectSql(), new Parameter(dh.ParamChar + "currentPropertyAsEqual_right", rightValue));
                        //接下来这里要进行把value的类型转换为poster.PropertyType表示的类型。
                        var obj = Activator.CreateInstance(poster.PropertyType);
                        poster.PropertyType.GetMethod("SetListValue").Invoke(obj, new object[] { value });
                        prop.SetValue(model, obj, null);
                    }
                    else continue;
                }
            }

            pmm.Total = pm.Total;
            return pmm;
        }
        /// <summary>
        /// 通过实体的属性标记如：Cocon90.Lib.Dal.Rule.DataFromAttribute("UnitTab", "UnitName", "UnitId", "CUnitId")，自动联合多表查询的分页信息。泛型 T 可以使用实体对像的子类。
        /// 参数请依次传入：分页排序字段、是否升序、请求第几页数据、每页多少条数据、自定义Where子句，如果没有请传入NULL或者空字符串、依次传入自定义Where子句中的参数。参数中请使用@做为参数
        /// </summary>
        /// <param name="orderColumnName">分页排序字段</param>
        /// <param name="isAsc">是否升序</param>
        /// <param name="pageNumber">请求第几页数据</param>
        /// <param name="pageSize">每页多少条数据</param>
        /// <param name="customWhere">自定义Where子句，如果没有请传入NULL或者空字符串</param>
        /// <param name="paramKeyValue">依次传入自定义Where子句中的参数。参数中请使用@做为参数，如：new Parameter("@name","宋兴柱")</param>
        /// <returns></returns>
        public PagedModel<T> GetPagedInfoViewUseAttrbute(string orderColumnName, bool isAsc, int pageNumber, int pageSize, string customWhere, params Parameter[] paramKeyValue)
        {
            T model = new T();
            List<ModelPoster> modelPosterLst = new List<ModelPoster>();
            Dictionary<string, List<ColumnPoster>> dics = new Dictionary<string, List<ColumnPoster>>();
            string mainTableAlisa = AttributeParam.MainTableAlisa;
            foreach (var prop in model.GetType().GetProperties())
            {
                if (!prop.CanWrite) { continue; }
                var objs = prop.GetCustomAttributes(typeof(GetDataFrom), true);
                var modelObjs = prop.GetCustomAttributes(typeof(GetModelFrom), true);
                if (modelObjs != null && modelObjs.Length > 0) { modelPosterLst.Add(new ModelPoster { PropertyName = prop.Name, PropertyType = prop.PropertyType, ModelFromAttribute = modelObjs[0] as GetModelFrom }); }
                if (objs != null && objs.Length > 0)
                {
                    var gdf = objs[0] as GetDataFrom;
                    if (gdf != null)
                    {
                        if (gdf.IsTabNameIsNull()) { gdf.TableName = model.TableName; }
                        if (gdf.IsTabAlisaIsNull())
                        {
                            if (gdf.IsTableUnit)
                            {
                                if (gdf.TableName == model.TableName) { gdf.TableAlias = mainTableAlisa; }
                                else
                                {
                                    foreach (var item in dics.Keys)
                                    {
                                        if (item.Contains(" ") && item.Split(' ').Length == 2 && item.Split(' ')[0] == gdf.TableName)
                                        {
                                            gdf.TableAlias = item.Split(' ')[1];
                                            break;
                                        }
                                    }
                                    if (gdf.IsTabAlisaIsNull()) { gdf.TableAlias = "tmp_Tab_" + Guid.NewGuid().ToString("N").Substring(0, 5); }
                                }
                            }
                            else { gdf.TableAlias = "tmpTab_" + Guid.NewGuid().ToString("N").Substring(0, 5); }
                        }
                        if (gdf.IsColumnNameIsNull()) { gdf.ColumnName = prop.Name; }
                        if (dics.ContainsKey(gdf.GetTableNameWithAlisa()))
                        {
                            dics[gdf.GetTableNameWithAlisa()].Add(new ColumnPoster { ColumnName = prop.Name, DataFromAttribute = gdf });
                        }
                        else
                        {
                            var lst = new List<ColumnPoster>();
                            lst.Add(new ColumnPoster { ColumnName = prop.Name, DataFromAttribute = gdf });
                            dics.Add(gdf.GetTableNameWithAlisa(), lst);
                        }
                    }

                }
                else if (modelObjs == null || modelObjs.Length <= 0)//如果GetDataFrom属性和GetModelFrom属性标记都没不存在的话。
                {
                    if (dics.ContainsKey(model.TableName + " " + mainTableAlisa))
                    {
                        dics[model.TableName + " " + mainTableAlisa].Add(new ColumnPoster { ColumnName = prop.Name, DataFromAttribute = new GetDataFrom(model.TableName, mainTableAlisa, prop.Name, null, null, true) });
                    }
                    else
                    {
                        var lst = new List<ColumnPoster>();
                        lst.Add(new ColumnPoster { ColumnName = prop.Name, DataFromAttribute = new GetDataFrom(model.TableName, mainTableAlisa, prop.Name, null, null, true) });
                        dics.Add(model.TableName + " " + mainTableAlisa, lst);
                    }
                }

            }

            List<string> selectColumnLst = new List<string>();
            List<string> whereLst = new List<string>();
            foreach (var key in dics.Keys)
            {
                foreach (var item in dics[key])
                {
                    if (item.DataFromAttribute.IsTableUnit)
                    {//若是表联合
                        if (item.ColumnName == item.DataFromAttribute.ColumnName)
                        {
                            selectColumnLst.Add(item.DataFromAttribute.TableAlias + "." + item.DataFromAttribute.ColumnName);
                        }
                        else
                        {
                            selectColumnLst.Add(item.DataFromAttribute.TableAlias + "." + item.DataFromAttribute.ColumnName + " " + item.ColumnName);
                        }
                    }
                    else
                    {//若是先选择后赋值
                        if (!(item.DataFromAttribute.IsColumnAsEqual_leftIsNull() || item.DataFromAttribute.IsColumnAsEqual_rightIsNull()))//如果不存在左条件或者不存在右条件
                        {
                            selectColumnLst.Add("(" + SqlHelper.getTopOrLimitRecord(String.Format("select {0} from {1} {2} where {2}.{3}={4}.{5}", item.DataFromAttribute.ColumnName, item.DataFromAttribute.TableName, item.DataFromAttribute.TableAlias, item.DataFromAttribute.TableColumnAsEqual_left, AttributeParam.MainTableAlisa, item.DataFromAttribute.CurrentColumnAsEqual_right), 1) + ") " + item.ColumnName);
                        }
                        else { if (!item.DataFromAttribute.IsTableUnit) { throw new ModelAttrbuteException("当属性前的特性GetDataFrom指示查询时，左右条件不能为空！the model attrbute DataFromAttribute's Property TableColumnAsEqual_left And CurrentPropertyAsEqual_right can't ot be null or empty"); } }
                    }

                    if (!(item.DataFromAttribute.IsColumnAsEqual_leftIsNull() || item.DataFromAttribute.IsColumnAsEqual_rightIsNull()))//如果不存在左条件或者不存在右条件
                    {
                        if (item.DataFromAttribute.IsTableUnit)
                        {
                            var condition = item.DataFromAttribute.TableAlias + "." + item.DataFromAttribute.TableColumnAsEqual_left + "=" + mainTableAlisa + "." + item.DataFromAttribute.CurrentColumnAsEqual_right;
                            if (!whereLst.Contains(condition)) { whereLst.Add(condition); }
                        }
                    }
                    else { if (!item.DataFromAttribute.IsTableUnit) { throw new ModelAttrbuteException("当属性前的特性GetDataFrom指示查询时，左右条件不能为空！the model attrbute DataFromAttribute's Property TableColumnAsEqual_left And CurrentPropertyAsEqual_right can't ot be null or empty"); } }
                }
            }
            List<string> fromTableList = new List<string>();
            foreach (var item in dics.Keys)
            {
                if (item != null && item.Length > 0)
                {
                    if (dics[item][0].DataFromAttribute.IsTableUnit)
                    {
                        fromTableList.Add(item);
                    }
                }
            }
            var fromTable = String.Join(",", fromTableList.ToArray());//生成如：aaaTab at,bbbTab bt,cccTab ct
            var selectColumn = String.Join(",", selectColumnLst);
            var whereCondition = String.Join(" and ", whereLst);
            var where = whereCondition;
            if ((customWhere + "").Trim() != "") { if ((whereCondition + "").Trim() == "") { where = customWhere; } else { where = whereCondition + " and " + customWhere; } }
            return GetPagedInfoView(modelPosterLst, fromTable.Split(','), selectColumn.Split(','), orderColumnName, isAsc, pageNumber, pageSize, where, paramKeyValue);
        }
        /// <summary>
        /// 生成表到数据库，如果库中表已经存在，则追加列。appendSqlWhenCreate是首次建表后后要执行的语句。
        /// </summary>
        /// <param name="appendSqlWhenCreate"></param>
        /// <returns></returns>
        public virtual bool UpdateSchemaToDataBase(params SqlBatch[] appendSqlWhenCreate)
        {
            var ttype = typeof(T);
            var instence = (T)Activator.CreateInstance(ttype);
            var names = this.GetColumnNames(instence, true);
            var tableName = instence.TableName;
            List<ColumnInfo> modelColumnList = new List<ColumnInfo>();
            var props = ttype.GetProperties();
            foreach (var name in names)
            {
                var pp = props.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
                if (pp != null)
                {
                    var colAttrs = pp.GetCustomAttributes(typeof(ColumnAttribute), true);
                    ColumnAttribute colAttr = null;
                    if (colAttrs != null && colAttrs.Length > 0) { colAttr = (ColumnAttribute)colAttrs[0]; }
                    modelColumnList.Add(new ColumnInfo
                    {
                        Type = pp.PropertyType,
                        Name = pp.Name,
                        IsNotNull = (colAttr == null ? false : colAttr.IsNotNull),
                        DefaultValue = (colAttr == null ? null : colAttr.DefaultValue),
                        IsPrimaryKey = (colAttr == null ? false : colAttr.IsPrimaryKey)
                    });
                }
            }
            List<ColumnInfo> currentList = new List<ColumnInfo>();
            var tabSchema = this.SqlHelper.getTableSchema(tableName);
            foreach (var item in tabSchema.Columns)
            {
                currentList.Add(new ColumnInfo() { Name = item.ColumnName, DefaultValue = item.DefaultValue, IsPrimaryKey = item.IsPrimaryKey, IsNotNull = !item.IsNullable });
            }
            if (this.dh is Utility.SQLiteDataHelper || this.dh is Utility.SQLDataHelper || this.dh is Utility.PostgreSqlDataHelper || this.dh is Utility.MySqlDataHelper)
            {
                #region
                bool isCreateTable = false;
                List<SqlBatch> sb = new List<SqlBatch>();
                if (currentList.Count == 0 && modelColumnList.Count > 0)//如果没有任何列，则说明没表，则建表。
                {
                    var primeryKeyColumn = modelColumnList.Find(c => c.IsPrimaryKey);//因为带主键的列，必须在建表时添加，之后是无法追加的。所以要先找它。
                    int index = 0;
                    if (primeryKeyColumn != null) { index = modelColumnList.IndexOf(primeryKeyColumn); }
                    var ci = modelColumnList[index];
                    modelColumnList.RemoveAt(index);
                    isCreateTable = true;
                    StringBuilder createColumn = new StringBuilder();
                    createColumn.AppendLine(string.Format("{1} {2} {3} {4} {5},", sh.safeName(tableName), sh.safeName(ci.Name), this.SqlHelper.GetTypeToDbTypeMapping(ci.Type, 0), (ci.IsPrimaryKey ? "Primary Key" : ""), (ci.IsPrimaryKey ? " not null " : (ci.IsNotNull ? " not null " : " null ")), string.IsNullOrWhiteSpace(ci.DefaultValue) ? "" : ci.DefaultValue));
                    var withDefaultColumn = modelColumnList.FindAll(c => !string.IsNullOrWhiteSpace(c.DefaultValue));
                    foreach (var col in withDefaultColumn)
                    {
                        var i = modelColumnList.IndexOf(col);
                        createColumn.AppendLine(string.Format("{1} {2} {3} {4} {5},", sh.safeName(tableName), sh.safeName(col.Name), this.SqlHelper.GetTypeToDbTypeMapping(col.Type, 0), (col.IsPrimaryKey ? "Primary Key" : ""), (col.IsPrimaryKey ? " not null " : (col.IsNotNull ? " not null " : " null ")), string.IsNullOrWhiteSpace(col.DefaultValue) ? "" : col.DefaultValue));
                        modelColumnList.RemoveAt(i);
                    }
                    sb.Add(new SqlBatch { Sql = string.Format("CREATE TABLE {0}({1});", sh.safeName(tableName), createColumn.ToString().Trim().TrimEnd(",".ToCharArray())) });
                }
                modelColumnList.ForEach(ci =>
                {
                    var col = currentList.Find(c => c.Name.ToLower() == ci.Name.ToLower());
                    if (col == null)//如果未找到某列
                    {
                        sb.Add(new SqlBatch { Sql = string.Format("ALTER TABLE {0} ADD {1} {2} {3} {4} {5};", sh.safeName(tableName), sh.safeName(ci.Name), this.SqlHelper.GetTypeToDbTypeMapping(ci.Type, 0), (ci.IsPrimaryKey ? "Primary Key" : ""), (ci.IsPrimaryKey ? " not null " : (ci.IsNotNull ? " not null " : " null ")), string.IsNullOrWhiteSpace(ci.DefaultValue) ? "" : ci.DefaultValue) });
                    }
                    //if (!(col.DefaultValue == ci.DefaultValue && col.IsNotNull == ci.IsNotNull && col.IsPrimaryKey == ci.IsPrimaryKey && col.Type == ci.Type))
                    //{//如果某一项不相等
                    //    //因为Sqlite不支持修改
                    //}
                });
                if (isCreateTable && appendSqlWhenCreate != null && appendSqlWhenCreate.Length > 0)
                {
                    foreach (var item in appendSqlWhenCreate)
                    {
                        sb.Add(item);
                    }
                }
                if (sb.Count == 0) return true;
                return this.dh.execBatch(sb, true, true) > 0;
                #endregion
            }

            else return false;
        }

        /// <summary>
        /// 获取Insert插入语句
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual SqlBatch GetInsertSqlByModel(T model)
        {
            var cols = GetColumnNames(model, false);
            var insertSql = this.SqlHelper.getInsertSqlParam(model.TableName, cols);
            List<Parameter> paramlist = new List<Parameter>();
            foreach (var c in cols)
            {
                var obj = typeof(T).GetProperty(c).GetValue(model, null);
                paramlist.Add(new Parameter(dh.ParamChar + c, obj));
            }
            return new SqlBatch { Sql = insertSql, Params = paramlist.ToArray() };
        }
        /// <summary>
        /// 获取Update语句
        /// </summary>
        /// <param name="model"></param>
        /// <param name="primeryKeyValue"></param>
        /// <param name="isNullMeansNotChange"></param>
        /// <returns></returns>
        public virtual SqlBatch GetUpdateSqlByPrimeryKey(T model, object primeryKeyValue, bool isNullMeansNotChange = true)
        {
            string primeryKey = this.SqlHelper.getPrimeryKey(model.TableName);
            List<string> columnNames;
            if (isNullMeansNotChange)
            {
                columnNames = this.GetColumnNames(model, false);
            }
            else
            {
                columnNames = this.GetColumnNames(model, true);
                columnNames.Remove(primeryKey);
            }
            string text = Guid.NewGuid().ToString("N");
            string where = string.Format("{0}={2}{1}", primeryKey, text, dh.ParamChar);
            string updateSqlParam = SqlHelper.getUpdateSqlParam(model.TableName, columnNames, where);
            List<Parameter> list = new List<Parameter>();
            list.Add(new Parameter(dh.ParamChar + text, primeryKeyValue));
            foreach (string current in columnNames)
            {
                if (isNullMeansNotChange)
                {
                    list.Add(new Parameter(string.Format(dh.ParamChar + "{0}", current), typeof(T).GetProperty(current).GetValue(model, null)));
                }
                else
                {
                    object value = typeof(T).GetProperty(current).GetValue(model, null);
                    if (value == null)
                    {
                        list.Add(new Parameter(string.Format(dh.ParamChar + "{0}", current), DBNull.Value));
                    }
                    else
                    {
                        list.Add(new Parameter(string.Format(dh.ParamChar + "{0}", current), value));
                    }
                }
            }
            return new SqlBatch() { Sql = updateSqlParam, Params = list.ToArray() };
        }
        /// <summary>
        /// 取得Update语句
        /// </summary>
        /// <param name="model"></param>
        /// <param name="customWhereStirng"></param>
        /// <param name="isNullMeansNotChange"></param>
        /// <param name="whereParas"></param>
        /// <returns></returns>
        public virtual SqlBatch GetUpdateSqlByCustomWhere(T model, string customWhereStirng, bool isNullMeansNotChange = true, params Parameter[] whereParas)
        {
            List<string> columnNames;
            if (isNullMeansNotChange)
            {
                columnNames = this.GetColumnNames(model, false);
            }
            else
            {
                columnNames = this.GetColumnNames(model, true);
                string primeryKey = this.SqlHelper.getPrimeryKey(model.TableName);
                columnNames.Remove(primeryKey);
            }
            string updateSqlParam = SqlHelper.getUpdateSqlParam(model.TableName, columnNames, customWhereStirng);
            List<Parameter> list = new List<Parameter>();
            if (whereParas != null)
            {
                for (int i = 0; i < whereParas.Length; i++)
                {
                    Parameter item = whereParas[i];
                    list.Add(item);
                }
            }
            foreach (string current in columnNames)
            {
                if (isNullMeansNotChange)
                {
                    list.Add(new Parameter(string.Format(dh.ParamChar + "{0}", current), typeof(T).GetProperty(current).GetValue(model, null)));
                }
                else
                {
                    object value = typeof(T).GetProperty(current).GetValue(model, null);
                    if (value == null)
                    {
                        list.Add(new Parameter(string.Format(dh.ParamChar + "{0}", current), DBNull.Value));
                    }
                    else
                    {
                        list.Add(new Parameter(string.Format(dh.ParamChar + "{0}", current), value));
                    }
                }
            }
            return new SqlBatch() { Sql = updateSqlParam, Params = list.ToArray() };
        }


    }
}
