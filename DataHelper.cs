using Cocon90.Lib.Dal.Rule;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Transactions;

namespace Cocon90.Lib.Dal
{
    /// <summary>
    /// 数据库基本访问类，能常用数据库均通用
    /// </summary>
    public abstract class DataHelper : IDataHelper
    {

        #region IDataHelper 成员
        private string connectionString = string.Empty;
        public virtual string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
        #endregion

        #region 子类参与实现
        public abstract char ParamChar { get; }
        public abstract System.Data.Common.DbCommand createCommond(string tsqlParamed, CommandType commandType, params Parameter[] paramKeyAndValue);
        public abstract System.Data.Common.DbDataAdapter createDataAdapter(System.Data.Common.DbCommand selectCmd);
        public abstract System.Data.Common.DbParameter createParameter(string key, object value);
        public abstract System.Data.Common.DbConnection createConnection();
        #endregion

        #region 事务
        public virtual Tools.TransactionExcutor beginTransaction()
        {
            return new Tools.TransactionExcutor(this);
        }
        public virtual Tools.TransactionExcutor beginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            return new Tools.TransactionExcutor(this, isolationLevel);
        }
        #endregion

        #region 事务域

        public virtual object createTransactionScopeObject()
        {
            return new System.Transactions.TransactionScope();
        }
        public virtual object createTransactionScopeObject(Rule.TransactionScopeOption transactionScopeOption)
        {
            return new System.Transactions.TransactionScope((System.Transactions.TransactionScopeOption)((int)transactionScopeOption));
        }
        public virtual object createTransactionScopeObject(Rule.TransactionScopeOption transactionScopeOption, Rule.TransactionOptions transactionOptions)
        {
            return new System.Transactions.TransactionScope((System.Transactions.TransactionScopeOption)((int)transactionScopeOption), new System.Transactions.TransactionOptions { Timeout = transactionOptions.Timeout, IsolationLevel = (System.Transactions.IsolationLevel)((int)transactionOptions.IsolationLevel) });
        }
        public virtual System.Transactions.TransactionScope createTransactionScope()
        {
            return new System.Transactions.TransactionScope();
        }
        public virtual System.Transactions.TransactionScope createTransactionScope(System.Transactions.TransactionScopeOption transactionScopeOption)
        {
            return new System.Transactions.TransactionScope(transactionScopeOption);
        }
        public virtual System.Transactions.TransactionScope createTransactionScope(System.Transactions.TransactionScopeOption transactionScopeOption, System.Transactions.TransactionOptions transactionOptions)
        {
            return new System.Transactions.TransactionScope(transactionScopeOption, transactionOptions);
        }
        public bool disposeTransactionScope(System.Transactions.TransactionScope transactionScope, bool isCommit)
        {
            if (transactionScope == null) return false;
            try
            {
                if (isCommit)
                { transactionScope.Complete(); }
                transactionScope.Dispose();
                return true;
            }
            catch { return false; }
        }
        public bool disposeTransactionScopeObject(object transactionScopeObject, bool isCommit)
        {
            if (transactionScopeObject is System.Transactions.TransactionScope)
            {
                return disposeTransactionScope(transactionScopeObject as System.Transactions.TransactionScope, isCommit);
            }
            else return false;
        }
        #endregion

        #region 常用执行方法体
        public virtual int execBatch(IEnumerable<SqlBatch> sqlBatch, bool isCommit, bool allowThrowException = true)
        {
            int count = 0;
            using (var trans = this.beginTransaction())
            {
                try
                {
                    foreach (var sql in sqlBatch)
                    {
                        trans.execNoQuery(sql.Sql, sql.Params);
                        count += 1;
                    }
                    if (isCommit) { trans.Commit(); } else { trans.Rollback(); }
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    if (allowThrowException)
                    {
                        var sb = sqlBatch.ToList()[count];
                        var sql = string.Join("\r\n", sb.Sql + "|参数：" + (sb.Params == null ? "" : string.Join(",", sb.Params.ToList().ConvertAll(p => p.Name + "=" + p.Value))));
                        throw new Exception(string.Format("执行第{0}个SqlBatch时出错：{1}，异常是：{2}", count + 1, sql, ex.Message), ex);
                    }
                }
            }
            return count;

        }
        public virtual DbParameter[] createParamters(params object[][] keyValueArr)
        {
            List<DbParameter> lst = new List<DbParameter>();
            if (keyValueArr == null) return lst.ToArray();
            foreach (object[] item in keyValueArr)
            {
                if (item != null && item.Length >= 2)
                { lst.Add(createParameter(item[0] + "", item[1])); }
            }
            return lst.ToArray();
        }

        public virtual DbParameter[] createParamters(Dictionary<string, object> keyValueDic)
        {
            List<DbParameter> lst = new List<DbParameter>();
            if (keyValueDic == null) return lst.ToArray();
            foreach (string key in keyValueDic.Keys)
            {
                lst.Add(createParameter(key, keyValueDic[key]));
            }
            return lst.ToArray();
        }

        public virtual int execNoQuery(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            var cmd = createCommond(tsqlParamed, CommandType.Text, paramKeyAndValue);
            try
            {
                var temp = cmd.ExecuteNonQuery();
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                return temp;
            }
            catch (Exception ex)
            {
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                throw ex;
            }
        }

        public virtual int execNoQueryProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            var cmd = createCommond(storedProcedureName, CommandType.StoredProcedure, paramKeyAndValue);
            try
            {
                var temp = cmd.ExecuteNonQuery();
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                return temp;
            }
            catch (Exception ex)
            {
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                throw ex;
            }
        }
        public virtual object getScalar(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            var cmd = createCommond(tsqlParamed, CommandType.Text, paramKeyAndValue);
            try
            {
                var temp = cmd.ExecuteScalar();
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                return temp;
            }
            catch (Exception ex)
            {
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                throw ex;
            }

        }

        public virtual object getScalarProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            var cmd = createCommond(storedProcedureName, CommandType.StoredProcedure, paramKeyAndValue);
            try
            {
                var temp = cmd.ExecuteScalar();
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                return temp;
            }
            catch (Exception ex)
            {
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                throw ex;
            }
        }
        /// <summary>
        /// 对Bool类型进行转换，boolString为True的语句是："on","1","true","yes" 
        /// </summary>
        /// <returns></returns>
        private static bool toBool(string boolString)
        {
            boolString = boolString + "";
            if (boolString.ToLower() == "on" || boolString.ToLower() == "1" || boolString.ToLower() == "true" || boolString.ToLower() == "yes") return true;
            else return false;
        }

        public virtual bool getBoolean(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            return toBool(getScalar(tsqlParamed, paramKeyAndValue) + "");
        }
        /// <summary>
        ///  执行存储过程的查询操作 返回bool类型的第一行第一列，如果结果为空，则返回"false"。为True的语句是："on","1","true","yes" 
        /// </summary>
        public virtual bool getBooleanProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            return toBool(getScalarProc(storedProcedureName, paramKeyAndValue) + "");
        }

        public virtual System.Data.DataSet getDataSet(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {

            DataSet ds = new DataSet();
            var cmd = createCommond(tsqlParamed, CommandType.Text, paramKeyAndValue);
            var dap = createDataAdapter(cmd);
            try
            {
                dap.Fill(ds);
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                dap.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                dap.Dispose();
                throw ex;
            }
        }

        public virtual System.Data.DataSet getDataSetProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            DataSet ds = new DataSet();
            var cmd = createCommond(storedProcedureName, CommandType.StoredProcedure, paramKeyAndValue);
            var dap = createDataAdapter(cmd);
            try
            {
                dap.Fill(ds);
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                dap.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                if (cmd.Connection != null && cmd.Connection.State != ConnectionState.Closed) { cmd.Connection.Close(); cmd.Connection.Dispose(); }
                cmd.Dispose();
                dap.Dispose();
                throw ex;
            }

        }

        public virtual DateTime getDateTime(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            DateTime temp = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime.TryParse(getScalar(tsqlParamed, paramKeyAndValue) + "", out temp);
            return temp;
        }

        public virtual DateTime getDateTimeProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            DateTime temp = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            DateTime.TryParse(getScalarProc(storedProcedureName, paramKeyAndValue) + "", out temp);
            return temp;
        }

        public virtual float getFloat(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            float temp = -1;
            float.TryParse(getScalar(tsqlParamed, paramKeyAndValue) + "", out temp);
            return temp;
        }

        public virtual float getFloatProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            float temp = -1;
            float.TryParse(getScalarProc(storedProcedureName, paramKeyAndValue) + "", out temp);
            return temp;
        }

        public virtual int getNumber(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            int temp = -1;
            int.TryParse(getScalar(tsqlParamed, paramKeyAndValue) + "", out temp);
            return temp;
        }

        public virtual int getNumberProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            int temp = -1;
            int.TryParse(getScalarProc(storedProcedureName, paramKeyAndValue) + "", out temp);
            return temp;
        }

        public virtual long getLong(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            long temp = -1;
            long.TryParse(getScalar(tsqlParamed, paramKeyAndValue) + "", out temp);
            return temp;
        }

        public virtual long getLongProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            long temp = -1;
            long.TryParse(getScalarProc(storedProcedureName, paramKeyAndValue) + "", out temp);
            return temp;
        }

        public virtual string getString(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            return getScalar(tsqlParamed, paramKeyAndValue) + "";
        }

        public virtual string getStringProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            return getScalarProc(storedProcedureName, paramKeyAndValue) + "";
        }

        public virtual System.Data.DataTable getTable(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            DataTable dt = new DataTable();
            var ds = getDataSet(tsqlParamed, paramKeyAndValue);
            if (ds.Tables != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;

        }

        public virtual System.Data.DataTable getTableProc(string storedProcedureName, params Parameter[] paramKeyAndValue)
        {
            DataTable dt = new DataTable();
            var ds = getDataSetProc(storedProcedureName, paramKeyAndValue);
            if (ds.Tables != null && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }


        public virtual List<string> getListString(bool isDistinct, string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            List<string> list = new List<string>();
            var dt = getTable(tsqlParamed, paramKeyAndValue);
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) { return list; }
            foreach (DataRow row in dt.Rows)
            {
                if (isDistinct)
                {
                    if (!list.Contains(row[0] + "")) { list.Add(row[0] + ""); }
                }
                else { list.Add(row[0] + ""); }
            }
            return list;
        }
        public virtual List<string> getListString(string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            return getListString(false, tsqlParamed, paramKeyAndValue);
        }
        public virtual List<string> getListStringProc(bool isDistinct, string storedProcedureName, params Parameter[] parameterValues)
        {
            List<string> list = new List<string>();
            var dt = getTableProc(storedProcedureName, parameterValues);
            if (dt == null || dt.Columns.Count <= 0 || dt.Rows.Count <= 0) { return list; }
            foreach (DataRow row in dt.Rows)
            {
                if (isDistinct)
                {
                    if (!list.Contains(row[0] + "")) { list.Add(row[0] + ""); }
                }
                else { list.Add(row[0] + ""); }
            }
            return list;
        }
        public virtual List<string> getListStringProc(string storedProcedureName, params Parameter[] parameterValues)
        {
            return getListStringProc(false, storedProcedureName, parameterValues);
        }


        public virtual List<long> getListLong(bool isDistinct, long convertErrorDefaultValue, string tsqlParamed, params Parameter[] paramKeyAndValue)
        {

            List<long> lstLong = new List<long>();
            var list = getListString(isDistinct, tsqlParamed, paramKeyAndValue);
            foreach (var item in list)
            {
                long defaultValue = convertErrorDefaultValue;
                if (long.TryParse("" + item, out defaultValue))
                {
                    if (isDistinct) { if (!lstLong.Contains(defaultValue)) { lstLong.Add(defaultValue); } }
                    else { lstLong.Add(defaultValue); }
                }
                else
                {
                    if (isDistinct) { if (!lstLong.Contains(convertErrorDefaultValue)) { lstLong.Add(convertErrorDefaultValue); } }
                    else { lstLong.Add(convertErrorDefaultValue); }
                }
            }
            return lstLong;
        }
        public virtual List<long> getListLong(long convertErrorDefaultValue, string tsqlParamed, params Parameter[] paramKeyAndValue)
        {
            return getListLong(false, convertErrorDefaultValue, tsqlParamed, paramKeyAndValue);
        }
        public virtual List<long> getListLongProc(bool isDistinct, long convertErrorDefaultValue, string storedProcedureName, params Parameter[] parameterValues)
        {
            List<long> lstLong = new List<long>();
            var list = getListStringProc(isDistinct, storedProcedureName, parameterValues);
            foreach (var item in list)
            {
                long defaultValue = convertErrorDefaultValue;
                if (long.TryParse("" + item, out defaultValue))
                {
                    if (isDistinct) { if (!lstLong.Contains(defaultValue)) { lstLong.Add(defaultValue); } }
                    else { lstLong.Add(defaultValue); }
                }
                else
                {
                    if (isDistinct) { if (!lstLong.Contains(convertErrorDefaultValue)) { lstLong.Add(convertErrorDefaultValue); } }
                    else { lstLong.Add(convertErrorDefaultValue); }
                }
            }
            return lstLong;
        }
        public virtual List<long> getListLongProc(long convertErrorDefaultValue, string storedProcedureName, params Parameter[] parameterValues)
        {
            return getListLongProc(false, convertErrorDefaultValue, storedProcedureName, parameterValues);
        }
        public virtual Tools.SqlHelper getSqlHelper()
        {
            return new Tools.SqlHelper(this);
        }
        #endregion

    }
}
